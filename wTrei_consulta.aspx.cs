using Constantes;
using ObjetosdeBanco;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Treinamentos;
using ObjetosdeBancoOperacoes;


public partial class wTreinamentos_wTrei_consulta : Abseed.Pagina
{
    private master_GenericModerna oMaster;
    private object tsCargo_lbDescricao;

    protected void Page_Load(object sender, EventArgs e)
    {
        oMaster = (master_GenericModerna)this.Master;
        oMaster.ProgId = 547;
        oMaster.RegisterPostbackTrigger(btnSalvar);


        if (!Page.IsPostBack)
        {
            CarregaDrop();
        }

    }

    private void CarregaDrop()
    {
        Formulario.FormularioDB db = new Formulario.FormularioDB();
        List<Formulario.Formulario> lista = db.Carrega(null, "", "TITULO asc");
        dpFormulario.DataSource = lista;
        dpFormulario.DataValueField = "FormularioID";
        dpFormulario.DataTextField = "Titulo";
        dpFormulario.DataBind();
        dpFormulario.Items.Insert(0, new ListItem() { Text = "", Value = "0" });
    }



    protected void Page_PreInit(object sender, System.EventArgs e)
    {
        Theme = Ambiente.GenericTheme;
    }

    private void VerificaNivelUsuario()
    {
        //Pega a permissão na url
        string sPerm = Funcoes.Funcoes.VB.Trim(Request.QueryString["perm"]);
        if (!string.IsNullOrEmpty(sPerm) && Funcoes.Funcoes.VB.IsNumeric(sPerm))
        {
            if (Session["id"] == null || Session["id"] != null)
            {
                if (Session["id"].Equals("absis"))
                {
                    //Absis
                    ViewState["nivel"] = Convert.ToInt32(sPerm);
                }
                else if (Principal.oUser.Info(Session["id"].ToString()).TemPermissao(oMaster.ProgId, Convert.ToInt32(sPerm)))
                {
                    ViewState["nivel"] = Convert.ToInt32(sPerm);
                }
                else
                {
                    Response.Redirect("../home/erro.aspx?id=404&prog=" + oMaster.ProgId.ToString());
                }
            }
        }
    }

    protected void pnlPesquisa_Pesquisar(object sender, EventArgs e)
    {
        CarregaGrid();
    }

    protected void btExcluir_Click(object sender, EventArgs e)
    {
        Treinamentos.Treinamento treinamento = new Treinamentos.Treinamento();
        foreach (string lista in grdRegistros.CodigosSelecionados)
        {
            treinamento.TreinamentoID = int.Parse(lista);
        }
        if (treinamento.TreinamentoID > 0)
        {
            msgConfirmaExcluir.Exibir();
        }
        else
        {
            msgAlert.Texto = "Selcione ao menos uma linha para a exclusão";
            msgAlert.Exibir();
        }
    }

    private bool ExcluirRegistros()
    {
        Banco.Conexao conexao = new Banco.Conexao();
        bool bRetorno = true;
     
        try
        {
            
            Treinamentos.TreinamentoDB treinamentoDB = new TreinamentoDB();
            conexao.AbreConexao();
            conexao.IniciaTransacao();
            Treinamentos.Treinamento treinamento = new Treinamentos.Treinamento();
            TreinamentoAlvo alvo = new TreinamentoAlvo();
            TreinamentoAlvoDB dB = new TreinamentoAlvoDB();
            alvo.TreinamentoID = treinamento.TreinamentoID;
            

            foreach (string lista in grdRegistros.CodigosSelecionados)
            {
                treinamento.TreinamentoID = int.Parse(lista);
                alvo.TreinamentoID = treinamento.TreinamentoID;
                bRetorno = dB.ExcluiPeloTreinamento(conexao, alvo.TreinamentoID);
                bRetorno = treinamentoDB.Exclui(conexao, treinamento);
                bRetorno = Avaliacao.Anexo.ExcluirDocumento(alvo.TreinamentoID.ToString());
                
                if (!bRetorno)
                {
                    break;
                }
            }
            if (bRetorno)
            {
                conexao.TerminaTransacao();
            }
            else
            {
                conexao.DesfazTransacao();
            }
        }
        catch (Exception)
        {
            msgAlert.Texto = "Erro ao excluir";
            msgAlert.Exibir();
            conexao.FechaConexao();
        }
        return bRetorno;
    }

    public void CarregaGrid()
    {
        DataTable oDt = MontaDataTable();
        grdRegistros.CarregaDados(oDt);
        if (oDt.Rows.Count == 0)
        {
            msgAlert.Texto = "Não foi encontrado nenhum registro com os filtros selecionados.";
            msgAlert.Exibir();
        }
    }

    private string MontaFiltro()
    {
        string sFiltro = string.Empty;
        
        if (!string.IsNullOrWhiteSpace(txtPesqTitulo.Texto))
        {
            if (sFiltro.Length > 0) sFiltro += " and ";
            sFiltro += "#Titulo = '" + txtPesqTitulo.Texto + "'";
        }
        if (!string.IsNullOrWhiteSpace(txtPesqPeriodo.SDataInicial) && !string.IsNullOrWhiteSpace(txtPesqPeriodo.SDataInicial))
        {
            if (sFiltro.Length > 0) sFiltro += " and ";
            sFiltro += "(#DataFinal >= '" + txtPesqPeriodo.SDataInicial + "' and #DataInicial <= '" + txtPesqPeriodo.SDataFinal + "')";
        }
        else if (!string.IsNullOrWhiteSpace(txtPesqPeriodo.SDataInicial))
        {
            if (sFiltro.Length > 0) sFiltro += " and ";
            sFiltro += "#DataFinal >= '" + txtPesqPeriodo.SDataInicial + "'";
        }
        else if (!string.IsNullOrWhiteSpace(txtPesqPeriodo.SDataFinal))
        {
            if (sFiltro.Length > 0) sFiltro += " and ";
            sFiltro += "#DataInicial <= '" + txtPesqPeriodo.SDataFinal + "'";
        }
        return sFiltro;
    }

    private DataTable MontaDataTable()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, MontaFiltro());
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    protected void msgConfirmaExcluir_Confirmado(object sender, EventArgs e)
    {
        ExcluirRegistros();
        CarregaGrid();
        msgAlert.Visible = false;
    }

    protected void msgConfirmaExcluir_Cancelado(object sender, EventArgs e)
    {
        msgAlert.Texto = "Você cancelou a exclusão";
        msgAlert.Exibir();
        CarregaGrid();
    }

    protected void grdRegistros_EditarItem(string sId)
    {
        HabilitaDiv(true);
        PreencheCamposModal(sId);

    }

    //método usado para preencher campos da modal com os valores do grid após que vem carregado conforme o retorno da pesquisa.
    protected void PreencheCamposModal(string sId)
    {
        Banco.Conexao conexao = new Banco.Conexao();
        conexao.AbreConexao();
        conexao.IniciaTransacao();
        TreinamentoDB db = new TreinamentoDB();
        Treinamentos.Treinamento trei = new Treinamentos.Treinamento();

        trei.TreinamentoID = int.Parse(sId);
        if (db.Carrega(conexao, ref trei))
        {
            
            hfId.Value = sId;
            txtCodigo.Texto = trei.Codigo;
            txtTitulo.Texto = trei.Titulo;
            txtLinkVideo.Texto = trei.LinkVideo;
            txtPrazo.Texto = trei.Prazo.ToString();
            txtCargaHoraria.Texto = trei.CargaHoraria;
            txtDataInicial.SData = trei.DataInicial.ToString("yyyyMMdd");
            txtDataFinal.SData = trei.DataFinal.ToString("yyyyMMdd");
            psOrientador.Texto = trei.Orientador;
            psInstrutor.Texto = trei.Instrutor;
            dpFormulario.SelectedValue = trei.FormularioID.ToString();
            List<TreinamentoAlvo> lista = new TreinamentoAlvoDB().CarregaPeloTreinamento(trei.TreinamentoID);
            DataTable oDt = CriaDataTable();
            Rotinas.Rotinas oUteis = new Rotinas.Rotinas();

            foreach (TreinamentoAlvo item in lista)
            {
                oDt.Rows.Add(item.Cargo, item.CargoTitulo, item.Lotacao, item.LotacaoTitulo, item.Estab, item.EstabTitulo);
            }
            grdModal.CarregaDados(oDt);
            ViewState["listaTreinamentos"] = oDt;
        }
        else
        {
            //Aviso de que  não encontrou o objeto no banco de dados com este id.
            msgAlert.Texto = "Aviso erro ao consultar base de dados.";
            msgAlert.Exibir();
        }
        conexao.FechaConexao();

    }
    //função para validar campos do modal
    private bool ValidaCampos()
    {
        string sMsg = string.Empty;
        if (string.IsNullOrWhiteSpace(txtCodigo.Texto))
            msgAlertModal.Texto += "O Código é obrigatório. ";

        if (string.IsNullOrWhiteSpace(txtTitulo.Texto))
            msgAlertModal.Texto += " O Título é obrigatório. ";
        if (string.IsNullOrWhiteSpace(txtDataInicial.SData))
            msgAlertModal.Texto += " A Data Inicial é obrigatória. ";
        else
        {
            DateTime ini = Funcoes.Funcoes.TextoemData(txtDataInicial.SData);
            if (ini.Date < DateTime.Now.Date)
                msgAlertModal.Texto += " A Data Inicial não pode conter data anteiror a data atual. ";
        }
        if (string.IsNullOrWhiteSpace(txtDataFinal.Texto))
            msgAlertModal.Texto += " A Data Final é obrigatória. ";
        else
        {
            if (!string.IsNullOrWhiteSpace(txtDataInicial.SData))
            {
                DateTime ini = Funcoes.Funcoes.TextoemData(txtDataInicial.SData);
                DateTime fim = Funcoes.Funcoes.TextoemData(txtDataFinal.SData);
                if (fim < ini)
                    msgAlertModal.Texto += " A Data Final não pode conter data anteiror a data inicial. ";
            }
        }
        /* //Opção usando VB um pouco mais verboso mas também funciona
         string sDataHoje = Funcoes.Funcoes.DatadeHoje(true, true);
         string sPrazo = dPrazo.SData;
         if (Funcoes.Funcoes.VB.StringMenor(sPrazo,sDataHoje)) //sPrazo < sDataHoje
             sMsg += "data invãlida";
             */
        if (txtPrazo.EstaVazio)
            msgAlertModal.Texto += " O Prazo é obrigatório. ";
        else
        {
            if (txtPrazo.Texto.Equals("0") || txtPrazo.Texto.Equals("00") || txtPrazo.Texto.Equals("000"))
                msgAlertModal.Texto += " O Prazo está inválido. ";
        }
        if (string.IsNullOrWhiteSpace(txtCargaHoraria.SHora))
            msgAlertModal.Texto += " A Carga Horária é obrigatória. ";

        if (string.IsNullOrWhiteSpace(msgAlertModal.Texto))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    protected void AdicionaTreinamentoModalBD()
    {
        bool bEdita = false;
        bool bResultado = false;
        if (!string.IsNullOrWhiteSpace(hfId.Value))
        {
            bEdita = true;
        }

        Banco.Conexao conexao = new Banco.Conexao();
        conexao.AbreConexao();
        TreinamentoDB dB = new TreinamentoDB();
        TreinamentoAlvoDB tcargoDb = new TreinamentoAlvoDB();
        Treinamentos.Treinamento trei = new Treinamentos.Treinamento();

        if (bEdita)//se bEdita True ele altera as informações editadas
            trei.TreinamentoID = int.Parse(hfId.Value);

        trei.Codigo = txtCodigo.Texto;
        trei.Titulo = txtTitulo.Texto;
        trei.DataInicial = Funcoes.Funcoes.TextoemData(txtDataInicial.SData);
        trei.DataFinal = Funcoes.Funcoes.TextoemData(txtDataFinal.SData);
        trei.LinkVideo = txtLinkVideo.Texto;
        trei.Orientador = psOrientador.Texto;
        trei.Instrutor = psInstrutor.Texto;
        trei.Prazo = int.Parse(txtPrazo.Texto);
        trei.CargaHoraria = txtCargaHoraria.Texto;
        trei.FormularioID = int.Parse(dpFormulario.SelectedValue);
        string caminhoArquivo = hdCaminho.Value.ToString();


        if (!string.IsNullOrEmpty(hdCaminho.Value.ToString()))
        {
            FileUpload upload = new FileUpload();

            upload.SaveAs(Constantes.Ambiente.CaminhoSite + "temp\\" + caminhoArquivo);

            Avaliacao.Anexo.AdicionaDocumento(trei.TreinamentoID.ToString(), Server.MapPath("../temp/" + caminhoArquivo));

            
        }

        conexao.IniciaTransacao();
        if (bEdita)
        {

            bResultado = dB.Altera(conexao, trei);

            bResultado = tcargoDb.ExcluiPeloTreinamento(conexao, trei.TreinamentoID);
        }
        else//se bEdita for false ele começa o processo para gravar novas informações no DB.
        {
            bResultado = dB.Inclui(conexao, trei);
        }

        if (bResultado)
        {
            DataTable oDt = ViewState["listaTreinamentos"] as DataTable;
            foreach (DataRow row in oDt.Rows)
            {
                TreinamentoAlvo tc = new TreinamentoAlvo();
                tc.TreinamentoID = trei.TreinamentoID;
                tc.Cargo = row["CARGO"].ToString();
                tc.Lotacao = row["LOTACAO"].ToString();
                tc.Estab = row["ESTAB"].ToString();
                bResultado = tcargoDb.Inclui(conexao, tc);
                if (!bResultado)
                    break;
            }
            if (!string.IsNullOrEmpty(hdCaminho.Value.ToString()))
            {
                FileUpload upload = new FileUpload();

               

                upload.SaveAs(Constantes.Ambiente.CaminhoSite + "temp\\" + caminhoArquivo);

                Avaliacao.Anexo.AdicionaDocumento(trei.TreinamentoID.ToString(), Server.MapPath("../temp/" + caminhoArquivo));


                //lblChecklist.Text = "Arquivo salvo com sucesso." + ArquivoUpLoad;
            }
        }

        //adicionar condicional se bResultado for true grava se bResultado for false mostra mensagem de erro entre cabeçario e campo código, e mantém campos preenchidos.
        if (bResultado)
        {
            conexao.TerminaTransacao();
            MsgConfirma.Texto = "Registro salvo com sucesso.";
            MsgConfirma.Exibir();
            CarregaGrid();
            HabilitaDiv(false);
            LimpaCamposModal();
        }
        else
        {
            conexao.DesfazTransacao();
            msgModalConfirmacaoErro.Texto = "Não foi possivel salvar registro.";
            msgModalConfirmacaoErro.Exibir();
            CarregaGrid();
            HabilitaDiv(false);
            LimpaCamposModal();
        }

        conexao.FechaConexao();

    }

    public bool AdicionaCargoModalParaDB(int iTreinamentoID)
    {
        Banco.Conexao conexao = new Banco.Conexao();
        Treinamentos.Treinamento trei = new Treinamentos.Treinamento();

        TreinamentoAlvoDB dB = new TreinamentoAlvoDB();
        bool bRetorno = true;
        List<TreinamentoAlvo> aux = dB.Carrega(conexao, "#TreinamentoID = " + iTreinamentoID.ToString());
        foreach (TreinamentoAlvo item in aux)
        {
            bRetorno = dB.Inclui(conexao, item);
            if (!bRetorno)
                break;
        }
        return bRetorno;
    }

    private int ContarLinhasCargo()
    {
        DataTable oDt = ViewState["listaTreinamentos"] as DataTable;
        if (oDt != null)
            return oDt.Rows.Count;
        return grdModal.TotalItens;
    }

    private DataTable CriaDataTable()
    {
        DataTable oDt = new DataTable();
        oDt.Columns.Add("CARGO");
        oDt.Columns.Add("CARGOTIT");
        oDt.Columns.Add("LOTACAO");
        oDt.Columns.Add("LOTACAOTIT");
        oDt.Columns.Add("ESTAB");
        oDt.Columns.Add("ESTABTIT");
        return oDt;
    }

    private bool ValidaAlvo()
    {
        return !tsCargo.CodigoEncontrado || !tsLotacao.CodigoEncontrado || !tsEstab.CodigoEncontrado;
    }

    private void AdicionarAlvoModal()
    {
        Treinamentos.TreinamentoAlvo alvo = new Treinamentos.TreinamentoAlvo();
        alvo.Cargo = tsCargo.CodigoSelecionado;
        alvo.Lotacao = tsLotacao.CodigoSelecionado;
        alvo.Estab = tsEstab.CodigoSelecionado;
        DataTable oDt = ViewState["listaTreinamentos"] as DataTable;
        if (oDt == null)
            oDt = CriaDataTable();

        oDt.Rows.Add(alvo.Cargo, alvo.CargoTitulo, alvo.Lotacao, alvo.LotacaoTitulo, alvo.Estab, alvo.EstabTitulo);
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
        LimparAlvo();
    }

    private void LimparAlvo()
    {
        tsCargo.Limpar();
        tsLotacao.Limpar();
        tsEstab.Limpar();
    }

    protected void btnAddAlvo_Click(object sender, EventArgs e)
    {
        if (ValidaAlvo())
            AdicionarAlvoModal();
        else
            msgAlertaAlvo.Exibir();
    }


    protected void btnExcluirAlvo_Click(object sender, EventArgs e)
    {
        ExcluirLinhaGrdModal();

    }

    protected void btAdicionar_Click(object sender, EventArgs e)
    {
        HabilitaDiv(true);
        LimpaCamposModal();

    }

    private void HabilitaDiv(bool bMostra)
    {
        divInclusao.Visible = bMostra;
        divFundo.Visible = bMostra;

    }

    protected void btnSair_Click(object sender, EventArgs e)
    {
        HabilitaDiv(false);
        CarregaGrid();
        LimpaCamposModal();
    }

    protected void LimpaCamposModal()
    {
        txtCodigo.Texto = string.Empty;
        txtTitulo.Texto = string.Empty;
        txtLinkVideo.Texto = string.Empty;
        txtPrazo.Texto = string.Empty;
        psInstrutor.Texto = string.Empty;
        psInstrutor.Texto = string.Empty;
        txtDataFinal.Limpar();
        txtDataInicial.Limpar();
        txtCargaHoraria.Limpar();
        hfId.Value = string.Empty;
        dpFormulario.SelectedValue = "0";
        DataTable oDt = CriaDataTable();
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
        LimparAlvo();
    }

    protected void btnSalvar_Click(object sender, EventArgs e)
    {
        if (ValidaCampos())
        {
            AdicionaTreinamentoModalBD();
            HabilitaDiv(false);
            LimpaCamposModal();
        }
        else
        {
            msgAlertModal.Exibir();
        }
    }

    private void ExcluirLinhaGrdModal()
    {
        DataTable oDt = ViewState["listaTreinamentos"] as DataTable;
        TreinamentoAlvo treiCargo = new TreinamentoAlvo();
        if (oDt != null)
        {
            foreach (string lista in grdModal.CodigosSelecionados)
            {
                string[] aux = lista.Split(';');
                DataRow row = Funcoes.Funcoes.DataTablePegaColunaComValor(oDt, aux[0], "CARGO", aux[1], "LOTACAO", aux[2], "ESTAB");
                oDt.Rows.Remove(row);
            }
        }
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
    }

    protected void grdRegistros_LinhaCriada(DataRow oDr, TableRow linha)
    {
        var formId = oDr["FORMULARIOID"];
        if (formId != null && !formId.Equals("0"))
            linha.Cells[8].Text = "Sim";
        else
            linha.Cells[8].Text = "Não";

    }
}


