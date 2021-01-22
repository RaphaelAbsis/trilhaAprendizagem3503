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
        limparFiltros();
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
            TreinamentoCargo cargo = new TreinamentoCargo();
            TreinamentoCargoDB dB = new TreinamentoCargoDB();
            cargo.TreinamentoID = treinamento.TreinamentoID;
            foreach (string lista in grdRegistros.CodigosSelecionados)
            {
                treinamento.TreinamentoID = int.Parse(lista);
                cargo.TreinamentoID = treinamento.TreinamentoID;
                bRetorno = dB.ExcluiPeloTreinamento(conexao, cargo.TreinamentoID);
                bRetorno = treinamentoDB.Exclui(conexao, treinamento);
                bRetorno = Avaliacao.Anexo.ExcluirDocumento(cargo.TreinamentoID.ToString());
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
        DataTable consultaDTTodosFiltros = MontarDataTablePorTodosFiltros();
        DataTable consultaDTTodosTituloCodigo = MontarDataTablePeloTituloCodigo();
        DataTable consultaDTTodosTituloPrazo = MontarDataTablePeloTituloPrazo();
        DataTable consultaDTTodosCodigoPrazo = MontarDataTablePeloCodigoPrazo();
        DataTable consultaDTCodigo = MontarDataTablePeloCodigo();
        DataTable consultaDTTitulo = MontarDataTablePeloTitulo();
        DataTable consultaDTTodosPrazo = MontarDataTablePeloPrazo();
        DataTable consultaDTVazio = MontarDataTablePeloVazio();
        if (consultaDTTodosFiltros.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && !string.IsNullOrWhiteSpace(cpTitulo.Texto) && !dPrazo.EstaVazio)
        {
            grdRegistros.CarregaDados(consultaDTTodosFiltros);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTodosTituloCodigo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpTitulo.Texto) && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && dPrazo.EstaVazio)
        {
            grdRegistros.CarregaDados(consultaDTTodosTituloCodigo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTodosTituloPrazo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpTitulo.Texto) && !dPrazo.EstaVazio && string.IsNullOrWhiteSpace(cpCodigo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTTodosTituloPrazo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTodosCodigoPrazo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && !dPrazo.EstaVazio && string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTTodosCodigoPrazo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTCodigo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && string.IsNullOrWhiteSpace(cpTitulo.Texto) && dPrazo.EstaVazio)
        {
            grdRegistros.CarregaDados(consultaDTCodigo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTitulo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpTitulo.Texto) && string.IsNullOrWhiteSpace(cpCodigo.Texto) && dPrazo.EstaVazio)
        {
            grdRegistros.CarregaDados(consultaDTTitulo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTodosPrazo.Rows.Count > 0 && !dPrazo.EstaVazio && string.IsNullOrWhiteSpace(cpCodigo.Texto) && string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTTodosPrazo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTVazio.Rows.Count > 0 && string.IsNullOrWhiteSpace(cpCodigo.Texto) && string.IsNullOrWhiteSpace(cpTitulo.Texto) && dPrazo.EstaVazio)
        {
            grdRegistros.CarregaDados(consultaDTVazio);
            pnlRegistros.Visible = true;
        }
        else
        {
            grdRegistros.CarregaDados(consultaDTVazio);
            pnlRegistros.Visible = true;
            msgAlert.Texto = "Não foi encontrado nenhum registro com os filtros selecionados.";
            msgAlert.Exibir();
        }
    }

    private DataTable MontarDataTablePorTodosFiltros()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "' and #Codigo = '" + cpCodigo.Texto + "' and #Prazo = '" + dPrazo.SData + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloTituloCodigo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "' and #Codigo = '" + cpCodigo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloTituloPrazo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "' and #Prazo = '" + dPrazo.SData + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloCodigoPrazo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Codigo = '" + cpCodigo.Texto + "' and #Prazo = '" + dPrazo.SData + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloCodigo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Codigo = '" + cpCodigo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloTitulo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloPrazo()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "#Prazo = '" + dPrazo.SData + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloVazio()
    {
        TreinamentoDB db = new TreinamentoDB();
        List<Treinamentos.Treinamento> lista = db.Carrega(null, "");
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
            txtCampo_Titulo.Texto = trei.Titulo;
            txtLinkVideo.Texto = trei.LinkVideo;
            txtPrazo.Texto = trei.Prazo.ToString("dd/MM/yyyy");
            psOrientador.Texto = trei.Orientador;
            psInstrutor.Texto = trei.Instrutor;
            dpFormulario.SelectedValue = trei.FormularioID.ToString();
            List<TreinamentoCargo> lista = new TreinamentoCargoDB().CarregaPeloTreinamento(trei.TreinamentoID);
            DataTable oDt = new DataTable();
            oDt.Columns.Add("CODIGO");
            oDt.Columns.Add("TITULO");
            Rotinas.Rotinas oUteis = new Rotinas.Rotinas();

            foreach (TreinamentoCargo item in lista)
            {
                oDt.Rows.Add(item.Cargo, oUteis.PegaTabe(1, "040", item.Cargo));
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
            msgAlertModal.Texto += "Campo código é obrigatório. ";

        if (string.IsNullOrWhiteSpace(txtCampo_Titulo.Texto))
            msgAlertModal.Texto += " Campo título é obrigatório. ";

        /* //Opção usando VB um pouco mais verboso mas também funciona
         string sDataHoje = Funcoes.Funcoes.DatadeHoje(true, true);
         string sPrazo = dPrazo.SData;
         if (Funcoes.Funcoes.VB.StringMenor(sPrazo,sDataHoje)) //sPrazo < sDataHoje
             sMsg += "data invãlida";
             */
        if (txtPrazo.EstaVazio)
        {

            msgAlertModal.Texto += " Campo prazo é obrigatório. ";
        }
        //Opção 2
        else
        {

            DateTime dtPrazo = Funcoes.Funcoes.TextoemData(txtPrazo.SData);
            if (dtPrazo.Date < DateTime.Now)
            {
                msgAlertModal.Texto += " Campo prazo não pode conter data anteiror a data atual. ";

            }
        }

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
        TreinamentoCargoDB tcargoDb = new TreinamentoCargoDB();
        Treinamentos.Treinamento trei = new Treinamentos.Treinamento();

        if (bEdita)//se bEdita True ele altera as informações editadas
            trei.TreinamentoID = int.Parse(hfId.Value);

        trei.Codigo = txtCodigo.Texto;
        trei.Titulo = txtCampo_Titulo.Texto;
        trei.LinkVideo = txtLinkVideo.Texto;
        trei.Orientador = psOrientador.Texto;
        trei.Instrutor = psInstrutor.Texto;
        trei.Prazo = Funcoes.Funcoes.TextoemData(txtPrazo.SData);
        trei.FormularioID = int.Parse(dpFormulario.SelectedValue);
       

        if (bEdita)//editar true ele edita informações
        {

            bResultado = dB.Altera(conexao, trei);
            bResultado = tcargoDb.ExcluiPeloTreinamento(conexao, trei.TreinamentoID);
            if (flpChecklist.HasFile)
            {

                flpChecklist.SaveAs(Constantes.Ambiente.CaminhoSite + "temp\\" + flpChecklist.FileName);

                Avaliacao.Anexo.AdicionaDocumento(trei.TreinamentoID.ToString(), Server.MapPath("../temp/" + flpChecklist.FileName));
                
            }

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
                TreinamentoCargo tc = new TreinamentoCargo();
                tc.TreinamentoID = trei.TreinamentoID;
                tc.Cargo = row["CODIGO"].ToString();
                tcargoDb.Inclui(conexao, tc);
               
            }
            if (flpChecklist.HasFile)
            {

                flpChecklist.SaveAs(Constantes.Ambiente.CaminhoSite + "temp\\" + flpChecklist.FileName);

                Avaliacao.Anexo.AdicionaDocumento(trei.TreinamentoID.ToString(), Server.MapPath("../temp/" + flpChecklist.FileName));


                //lblChecklist.Text = "Arquivo salvo com sucesso." + ArquivoUpLoad;
            }
        }

        //adicionar condicional se bResultado for true grava se bResultado for false mostra mensagem de erro entre cabeçario e campo código, e mantém campos preenchidos.
        if (bResultado)
        {
            MsgConfirma.Texto = "Registro salvo com sucesso.";
            MsgConfirma.Exibir();
            CarregaGrid();
            HabilitaDiv(false);
            LimpaCamposModal();
        }
        else
        {
            msgModalConfirmacaoErro.Texto = "Erro Não foi possivel salvar registro.";
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

        TreinamentoCargoDB dB = new TreinamentoCargoDB();
        bool bRetorno = true;
        List<TreinamentoCargo> aux = dB.Carrega(conexao, "#TreinamentoID = " + iTreinamentoID.ToString());
        foreach (TreinamentoCargo item in aux)
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

    private void AdicionaCargoGridModal()
    {   
        Treinamentos.Treinamento trei = new Treinamentos.Treinamento();
        trei.Codigo = tsCargo.CodigoSelecionado;
        string codigo = tsCargo.Sucinta;//recebe valor da string que é carregada ao lado do campo código
        DataTable oDt = ViewState["listaTreinamentos"] as DataTable;
        if (oDt == null)
        {
            oDt = new DataTable();
            oDt.Columns.Add("CODIGO");
            oDt.Columns.Add("TITULO");
        }
        oDt.Rows.Add(trei.Codigo, codigo);
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
        tsCargo.Limpar();

    }

    private void limparFiltros()
    {
        cpCodigo.Texto = "";
        cpTitulo.Texto = "";
        dPrazo.Texto = "";
    }

    protected void btnAddCargo_Click(object sender, EventArgs e)
    {
        AdicionaCargoGridModal();
       
    }

    protected void btnExcluir_Click(object sender, EventArgs e)
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
        txtCampo_Titulo.Texto = string.Empty;
        txtLinkVideo.Texto = string.Empty;
        txtPrazo.Texto = string.Empty;
        psInstrutor.Texto = string.Empty;
        psInstrutor.Texto = string.Empty;
        tsCargo.Texto = string.Empty;
        hfId.Value = string.Empty;
        dpFormulario.SelectedValue = "0";
        DataTable oDt = new DataTable();
        oDt.Columns.Add("CODIGO");
        oDt.Columns.Add("TITULO");
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
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
        TreinamentoCargo treiCargo = new TreinamentoCargo();
        if (oDt != null)
        {
            foreach (string lista in grdModal.CodigosSelecionados)
            {
                DataRow row = Funcoes.Funcoes.DataTablePegaColunaComValor(oDt, lista, "CODIGO");
                oDt.Rows.Remove(row);
            }
        }
        grdModal.CarregaDados(oDt);
        ViewState["listaTreinamentos"] = oDt;
        
    }
}
