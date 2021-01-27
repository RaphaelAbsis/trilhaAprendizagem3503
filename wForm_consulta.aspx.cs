using Constantes;
using Formulario;
using ObjetosdeBanco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class wFormulario_wForm_consulta : Abseed.Pagina
{
    private master_GenericModerna oMaster;
    protected void Page_Load(object sender, EventArgs e)
    {
        oMaster = (master_GenericModerna)this.Master;
        oMaster.ProgId = 545;
        Version Versao = new Version("1.0");
        tbcBotoes.Visible = true;
        if (!Page.IsPostBack)
        {
            VerificaNivelUsuario();
            CarregaGrid();
        }
    }
    

    protected void Page_PreInit(object sender, System.EventArgs e)
    {
        Theme = Ambiente.GenericTheme;
    }

    private void VerificaNivelUsuario()
    {
        //Pega a permissão na url
        string sPerm = Funcoes.Funcoes.VB.Trim(Request.QueryString["perm"]);
        if (!String.IsNullOrEmpty(sPerm) && Funcoes.Funcoes.VB.IsNumeric(sPerm))
        {
            if (Session["id"] != null)
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
            else
            {
                Response.Redirect("../home/erro.aspx?id=404&prog=" + oMaster.ProgId.ToString());
            }
        }
    }

    protected void pnlPesquisa_Pesquisar(object sender, EventArgs e)
    {
        CarregaGrid();
        msgAlert.Texto = "";
        tbcBotoes.Visible = true;
        
    }

    protected void btAdicionar_Click(object sender, EventArgs e)
    {
        Response.Redirect("wForm_manutencao.aspx");
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
            tbcBotoes.Visible = true;
            msgAlert.Texto = "Selecione ao menos uma linha para a exclusão";
            msgAlert.Exibir();
        }
    }

    public void CarregaGrid()
    {
        DataTable consultaDTDoisFiltros = MontarDataTablePeloDoisFiltros();
        DataTable consultaDTCodigo= MontarDataTablePeloCodigo();
        DataTable consultaDTTitulo = MontarDataTablePeloTitulo();
        DataTable consultaDTVazio = MontarDataTablePeloVazio();
        if (consultaDTDoisFiltros.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && !string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTDoisFiltros);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTCodigo.Rows.Count > 0 && !string.IsNullOrWhiteSpace(cpCodigo.Texto) && string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTCodigo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTTitulo.Rows.Count > 0 && string.IsNullOrWhiteSpace(cpCodigo.Texto) && !string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTTitulo);
            pnlRegistros.Visible = true;
        }
        else if (consultaDTVazio.Rows.Count > 0 && string.IsNullOrWhiteSpace(cpCodigo.Texto) && string.IsNullOrWhiteSpace(cpTitulo.Texto))
        {
            grdRegistros.CarregaDados(consultaDTVazio);
            pnlRegistros.Visible = true;
        }
        else
        {
            pnlRegistros.Visible = false;
            msgAlert.Texto = "Não foi encontrado nenhum registro com os filtros selecionados.";
            msgAlert.Exibir();
        }
    }

    private bool ExcluirRegistros()
    {
        Banco.Conexao conexao = new Banco.Conexao();
        bool bRetorno = true;
        try
        {
            Formulario.FormularioDB formularioDB = new FormularioDB();
            conexao.AbreConexao();
            conexao.IniciaTransacao();
            Formulario.Formulario formulario = new Formulario.Formulario();
            foreach (string lista in grdRegistros.CodigosSelecionados)
            {
                formulario.FormularioID = int.Parse(lista);
                bRetorno = formularioDB.Exclui(conexao, formulario);
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

    private DataTable MontarDataTablePeloDoisFiltros()
    {
        FormularioDB db = new FormularioDB();
        List<Formulario.Formulario> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "' and #Codigo = '" + cpCodigo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloCodigo()
    {
        FormularioDB db = new FormularioDB();
        List<Formulario.Formulario> lista = db.Carrega(null, "#Codigo = '" + cpCodigo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloTitulo()
    {
        FormularioDB db = new FormularioDB();
        List<Formulario.Formulario> lista = db.Carrega(null, "#Titulo = '" + cpTitulo.Texto + "'");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    private DataTable MontarDataTablePeloVazio()
    {
        FormularioDB db = new FormularioDB();
        List<Formulario.Formulario> lista = db.Carrega(null, "");
        DataTable oDT = FuncoesTypes.ConverteListaParaDataTable(lista);
        return oDT;
    }

    protected void msgConfirmaExcluir_Confirmado(object sender, EventArgs e)
    {
        ExcluirRegistros();
        CarregaGrid();
        msgAlert.Texto = "Excluído com sucesso!";
        msgAlert.Exibir();
        tbcBotoes.Visible = true;

    }

    protected void msgConfirmaExcluir_Cancelado(object sender, EventArgs e)
    {
        msgConfirmaExcluir.Visible = false;
        tbcBotoes.Visible = true;
    }

    protected void grdRegistros_EditarItem(string sId)
    {
        Response.Redirect("wForm_manutencao.aspx?prog=546&perm=0&fid="+sId);
    }
}