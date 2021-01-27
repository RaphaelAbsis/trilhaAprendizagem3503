using Constantes;
using Formulario;
using ObjetosdeBanco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class wFormulario_wForm_manutencao : Abseed.Pagina
{
    private master_GenericModerna oMaster;
    private const string CHAVE_QS_TREINAMENTOID = "trid";
    protected void Page_Load(object sender, EventArgs e)
    {
        oMaster = (master_GenericModerna)this.Master;
        oMaster.ProgId = 546;
        Version Versao = new Version("1.0");
        if (!Page.IsPostBack)
        {
            TemTreinamento();
            VerificaNivelUsuario();
            PegaValorDoFormConsulta();
            
        }
        else
        {
            MensagemErroTreinamento.Texto = "Formulario não encontrado";
            MensagemErroTreinamento.Exibir();
            pnlFormulario.Visible = false;
        }
        

    }

    private bool TemTreinamento()
    {
        return Request.QueryString[CHAVE_QS_TREINAMENTOID] != null;
    }

    int somarID = 0;

    protected void Page_PreInit(object sender, System.EventArgs e)
    {
        Theme = Ambiente.GenericTheme;
    }

    private void VerificaNivelUsuario()
    {
        //Pega a permissão na url
        
        if (!String.IsNullOrEmpty(CHAVE_QS_TREINAMENTOID) && Funcoes.Funcoes.VB.IsNumeric("perm"))
        {
            if (Session["id"] != null)
            {
                if (Session["id"].Equals("absis"))
                {
                    //Absis
                    ViewState["nivel"] = Convert.ToInt32("perm");
                }
                else if (Principal.oUser.Info(Session["id"].ToString()).TemPermissao(oMaster.ProgId, Convert.ToInt32("perm")))
                {
                    ViewState["nivel"] = Convert.ToInt32("perm");
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

    private TipoPergunta ConverterStringEmEnum(string sValor)
    {
        if (sValor.Equals("0"))
            return TipoPergunta.RespostaCurta;
        else if (sValor.Equals("1"))
            return TipoPergunta.RespostaLonga;
        else if (sValor.Equals("2"))
            return TipoPergunta.MultiplaEscolha;
        else if (sValor.Equals("3"))
            return TipoPergunta.CaixaDeSelecao;
        else if (sValor.Equals("4"))
            return TipoPergunta.ListaSuspensa;
        return TipoPergunta.RespostaCurta;
    }

    private void PegaValorDoFormConsulta()
    {
        string sTreinamentoId = Funcoes.Funcoes.VB.Trim(Request.QueryString["fid"]);
        Formulario.Formulario form = new Formulario.Formulario();
        FormularioDB db = new FormularioDB();
        if (string.IsNullOrEmpty(sTreinamentoId) || !Funcoes.Funcoes.VB.IsNumeric(sTreinamentoId))
        {
            MensagemErroTreinamento.Texto = "Formulario não encontrado";
            MensagemErroTreinamento.Exibir();
            pnlFormulario.Visible = false;
        }
        else
        {
            form.FormularioID = int.Parse(sTreinamentoId);
            if (db.Carrega(null, ref form))
            {
                PreencheCamposFormManutencao(form);
            }
        }
    }

    private void PreencheCamposFormManutencao(Formulario.Formulario form)
    {
        txtCodigo.Texto = form.Codigo;
        txtTituloFormulario.Texto = form.Titulo;
        txtDescricaoFormulario.Texto = form.Descricao;
    }

    protected void VerificaPreenche() {
        

    }

    private int PegaOrdem()
    {
        string sOrdem = hdOrdem.Value;
        if (string.IsNullOrWhiteSpace(sOrdem))
            sOrdem = "1";
        else
            sOrdem = (int.Parse(sOrdem) + 1).ToString();
        hdOrdem.Value = sOrdem;
        return int.Parse(sOrdem);
    }
    

    private List<Formulario.RespostaOpcao> AdicionarResposta()
    {
        //if (!string.IsNullOrWhiteSpace(txtTexto.Texto))// && !string.IsNullOrWhiteSpace(txtOrdem.Texto))
        //{
        List<Formulario.RespostaOpcao> listaRespostas = ViewState["ListaRespostas"] as List<Formulario.RespostaOpcao>;
        if (listaRespostas == null)
            listaRespostas = new List<Formulario.RespostaOpcao>();
        
        if (!string.IsNullOrWhiteSpace(txtTexto.Texto))
        {
            Formulario.RespostaOpcao respostaOpcao = new Formulario.RespostaOpcao();
            respostaOpcao.PerguntaID = int.Parse(hFIDResposta.Value);
            respostaOpcao.Texto = txtTexto.Texto;
            respostaOpcao.Ordem = PegaOrdem();
            respostaOpcao.RespostaCerta = chkRespostaCerta.Checked;
            listaRespostas.Add(respostaOpcao);
        }
        
        rptRespostaVista.DataSource = listaRespostas;
        rptRespostaVista.DataBind();
        ViewState["ListaRespostas"] = listaRespostas;
        return listaRespostas;
        //}
        //else if (string.IsNullOrWhiteSpace(txtTexto.Texto))
        //{
        //    msgAlert.Texto = "Campo resposta em branco";
        //    msgAlert.Exibir();
        //}
        //else if (string.IsNullOrWhiteSpace(txtOrdem.Texto))
        //{
        //    msgAlert.Texto = "Campo ordem em branco";
        //    msgAlert.Exibir();
        //}
        //else
        //{
        //    msgAlert.Texto = "Campo texto e ordem em brancos";
        //    msgAlert.Exibir();
        //}
        //return new List<Formulario.RespostaOpcao>();
    }

    private void GuardarPergunta()
    {
        bool bEdicao = !string.IsNullOrWhiteSpace(hFAdicionar.Value);
        List<Formulario.Pergunta> listaPerguntas = ViewState["ListaPerguntas"] as List<Formulario.Pergunta>;
        if (listaPerguntas == null)
            listaPerguntas = new List<Formulario.Pergunta>();

        Formulario.Pergunta pergunta = new Formulario.Pergunta();
        pergunta.PerguntaID = int.Parse(hFIDPergunta.Value);
        pergunta.Titulo = txtTituloPergunta.Texto;
        pergunta.Tipo = ConverterStringEmEnum(dplTipoPergunta.SelectedValue);
        pergunta.Obrigatoria = cObrigatoria.Checked;
        pergunta.Descricao = txtDescricaoPergunta.Texto;
        pergunta.RespostasOpcoesMemoria = AdicionarResposta();
        if (bEdicao)
        {
            int indice = int.Parse(hFAdicionar.Value);
            listaPerguntas[indice] = pergunta;
        }
        else
            listaPerguntas.Add(pergunta);
        rptPerguntas.DataSource = listaPerguntas;
        rptPerguntas.DataBind();
        ViewState["ListaPerguntas"] = listaPerguntas;
    }

    private void AlterarPergunta(Pergunta pergunta)
    {
        pergunta.Titulo = txtTituloPergunta.Texto;
        pergunta.Descricao = txtDescricaoPergunta.Texto;
        pergunta.Tipo = ConverterStringEmEnum(dplTipoPergunta.SelectedValue);
        pergunta.Obrigatoria = cObrigatoria.Checked;
        MostraResposta(dplTipoPergunta.SelectedValue);
        /*RespostaOpcao respostaOpcao = new RespostaOpcao();
        respostaOpcao.Texto = txtTexto.Texto;
        respostaOpcao.Ordem = int.Parse(txtOrdem.Texto);
        List<RespostaOpcao> listaRespostasOpcaos = ViewState["listaRespostasAlteradas"] as List<RespostaOpcao>;
        listaRespostasOpcaos.Add(respostaOpcao);
        ViewState["listaRespostasAlteradas"] = listaRespostasOpcaos;
        pergunta.RespostasOpcoesMemoria = listaRespostasOpcaos;*/
    }

    private void LimparPergunta()
    {
        txtTituloPergunta.Texto = "";
        txtDescricaoPergunta.Texto = "";
        dplTipoPergunta.SelectedValue = "";
        cObrigatoria.Checked = false;
        txtTexto.Texto = "";
        //txtOrdem.Texto = "";
        hdOrdem.Value = "";
        hFAdicionar.Value = "";
    }

    private void LimparRespostas()
    {
        txtTexto.Texto = "";
        //txtOrdem.Texto = "";
        chkRespostaCerta.Checked = false;
    }

    protected void btnAdicionarPergunta_Click(object sender, EventArgs e)
    {
        GuardarPergunta();
        LimparPergunta();
        ViewState["ListaRespostas"] = null;
        dInserirTextoResposta.Visible = false;
        rptRespostaVista.Visible = false;
        int somarIDResposta = int.Parse(hFIDResposta.Value);
        int somarIDPergunta = int.Parse(hFIDPergunta.Value);
        somarIDResposta++;
        somarIDPergunta++;
        hFIDResposta.Value = somarIDPergunta.ToString();
        hFIDPergunta.Value = somarIDPergunta.ToString();
    }

    protected void rptPerguntas_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Formulario.Pergunta perg = e.Item.DataItem as Formulario.Pergunta;
        if (perg.Tipo == Formulario.TipoPergunta.MultiplaEscolha)
        {
            var controle = (e.Item.FindControl("rdRespostas") as RadioButtonList);
            controle.DataSource = perg.RespostasOpcoesMemoria;
            controle.DataTextField = "Texto";
            controle.DataValueField = "Ordem";
            controle.DataBind();
            controle.Visible = true;
        }
        else if (perg.Tipo == Formulario.TipoPergunta.CaixaDeSelecao)
        {
            var controle = (e.Item.FindControl("chkRespostas") as CheckBoxList);
            controle.DataSource = perg.RespostasOpcoesMemoria;
            controle.DataTextField = "Texto";
            controle.DataValueField = "Ordem";
            controle.DataBind();
            controle.Visible = true;
        }
        else if (perg.Tipo == Formulario.TipoPergunta.ListaSuspensa)
        {
            var controle = (e.Item.FindControl("ddlRespostas") as DropDownList);
            controle.DataSource = perg.RespostasOpcoesMemoria;
            controle.DataTextField = "Texto";
            controle.DataValueField = "Ordem";
            controle.DataBind();
            controle.Visible = true;
        }
        else if (perg.Tipo == Formulario.TipoPergunta.RespostaCurta)
        {
            var controle = (e.Item.FindControl("txtRespostaCurta") as Abseed.Campo);
            controle.Visible = true;
        }
        else if (perg.Tipo == Formulario.TipoPergunta.RespostaLonga)
        {
            var controle = (e.Item.FindControl("txtRespostaLonga") as Abseed.Campo);
            controle.Visible = true;
        }
        if (perg.Obrigatoria)
        {
            var controle = (e.Item.FindControl("lblObrigatoria") as Label);
            controle.Visible = true;
        }
    }

    protected void dplTipoPergunta_SelectedIndexChanged(object sender, EventArgs e)
    {
        MostraResposta(dplTipoPergunta.SelectedValue);
        MontaRepeaterResposta();
    }

    private void MostraResposta(string sTipoPergunta)
    {
        if (sTipoPergunta.Equals("2") || sTipoPergunta.Equals("3") || sTipoPergunta.Equals("4"))
        {
            dInserirTextoResposta.Visible = true;
        }
    }

    protected void btnAdicionarOpcao_Click(object sender, EventArgs e)
    {
        AdicionarResposta();
        LimparRespostas();
        rptRespostaVista.Visible = true;
    }

    private string ConverterEnumEmString(string sValor)
    {
        if (sValor.Equals("RespostaCurta"))
            return "0";
        else if (sValor.Equals("RespostaLonga"))
            return "1";
        else if (sValor.Equals("MultiplaEscolha"))
            return "2";
        else if (sValor.Equals("CaixaDeSelecao"))
            return "3";
        else if (sValor.Equals("ListaSuspensa"))
            return "4";
        return "";
    }

    protected void btnEditar_Click(object sender, EventArgs e)
    {
        Button botao = (sender as Button);
        var indice = botao.CommandArgument;
        List<Formulario.Pergunta> listaPerguntas = ViewState["ListaPerguntas"] as List<Formulario.Pergunta>;
        var pergunta = listaPerguntas[int.Parse(indice)];
        txtTituloPergunta.Texto = pergunta.Titulo;
        txtDescricaoPergunta.Texto = pergunta.Descricao;
        dplTipoPergunta.SelectedValue = ConverterEnumEmString(pergunta.Tipo.ToString());
        cObrigatoria.Checked = pergunta.Obrigatoria;
        rptRespostaVista.DataSource = pergunta.RespostasOpcoesMemoria;
        rptRespostaVista.DataBind();
        rptRespostaVista.Visible = true;
        hdOrdem.Value = PegaOrdemDaLista(pergunta.RespostasOpcoesMemoria);
        ViewState["ListaRespostas"] = pergunta.RespostasOpcoesMemoria;
        hFAdicionar.Value = indice;
        btnAdicionarPergunta.Legenda = "Alterar";
        btnAdicionarPergunta.CorDoBotao = Abseed.CorBotao.Amarelo;
        AlterarPergunta(pergunta);
    }

    private string PegaOrdemDaLista(List<Formulario.RespostaOpcao> lista)
    {
        return lista.Max(x => x.Ordem).ToString();
    }

    private void MontaRepeaterResposta()
    {
        rptRespostaVista.DataSource = ViewState["ListaRespostas"];
        rptRespostaVista.DataBind();
        rptRespostaVista.Visible = true;
    }
}