<%@ Page Title="" Language="C#" MasterPageFile="~/master/GenericModerna.master" AutoEventWireup="true" CodeFile="wForm_manutencao.aspx.cs" Inherits="wFormulario_wForm_manutencao" %>

<%@ Register Assembly="Abseed" Namespace="Abseed" TagPrefix="absis" %>
<asp:Content ID="Content1" ContentPlaceHolderID="masterBody" runat="Server">
    <absis:Mensagem runat="server" ID="MensagemErroTreinamento"></absis:Mensagem>

    <absis:Painel ID="pnlFormulario" runat="server" Legenda="Cabeçalho">
        <asp:HiddenField ID="hFAdicionar" Value="" runat="server" />
        <asp:HiddenField ID="hFIDPergunta" Value="1" runat="server" />
        <asp:HiddenField ID="hFIDResposta" Value="1" runat="server" />
        <absis:Campo ID="txtCodigo" runat="server" Texto="" Legenda="Código"></absis:Campo>
        <br />
        <absis:Campo ID="txtTituloFormulario" runat="server" Texto="" Legenda="Título"></absis:Campo>
        <br />
        <absis:Campo ID="txtDescricaoFormulario" runat="server" Texto="" Legenda="Descrição" ModoTexto="MultiLine" Height="70px" Largura="300px" ></absis:Campo>
        
    </absis:Painel>
    <absis:Painel ID="pnlPergunta" runat="server" Legenda="Adicionar Pergunta">
        <absis:Coluna ID="colEsquerda" runat="server" ColunaTipo="Esquerda" LarguraFixa="50%">
            <absis:Campo ID="txtCodigoPergunta" runat="server" Texto="" Legenda="Código"></absis:Campo>
            <br />
            <absis:Campo ID="txtTituloPergunta" runat="server" Texto="" Legenda="Título"></absis:Campo>
            <br />
            <absis:Campo ID="txtDescricaoPergunta" runat="server" Texto="" Legenda="Descrição" ModoTexto="MultiLine" Height="70px" Largura="300px" ></absis:Campo>
            <br />
            <asp:Repeater runat="server" ID="rptRespostaVista">
                <ItemTemplate>
                    <div id="areaRespostaVista">
                        <absis:Etiqueta ID="eResposta" Texto='<%# DataBinder.Eval(Container.DataItem, "Texto") %>' runat="server"></absis:Etiqueta>
                        <absis:Etiqueta ID="lblRespostaCerta" Texto='<%# DataBinder.Eval(Container.DataItem, "RespostaCerta") %>' runat="server"></absis:Etiqueta>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div id="dInserirTextoResposta" runat="server" visible="false">
                <absis:Etiqueta ID="lblTexto" Texto="Resposta" runat="server"></absis:Etiqueta>
                <asp:HiddenField ID="hdOrdem" runat="server" />
                <absis:Campo ID="txtTexto" runat="server"></absis:Campo>
                <absis:Check ID="chkRespostaCerta" runat="server" Texto="Resposta Certa?" />
                <absis:Botao ID="btnAdicionarOpcao" Legenda="+" OnClick="btnAdicionarOpcao_Click" Style="float:right; margin-top:4px" runat="server"></absis:Botao>
                <%--<br />
                <absis:Etiqueta ID="lblOrdem" Texto="Ordem" runat="server"></absis:Etiqueta>
                <absis:Campo ID="txtOrdem" runat="server"></absis:Campo>--%>
            </div>
        </absis:Coluna>
        <absis:Coluna ID="colDireita" runat="server" ColunaTipo="Direita" LarguraFixa="50%">
            <absis:Etiqueta ID="eTipoPergunta" Texto="Tipo" runat="server"></absis:Etiqueta>
            <asp:DropDownList ID="dplTipoPergunta" AutoPostBack="true" OnSelectedIndexChanged="dplTipoPergunta_SelectedIndexChanged" class="abseed_dropdownlist" runat="server">
                <asp:ListItem Text="" Value="" />
                <asp:ListItem Text="Resposta Curta" Value="0" />
                <asp:ListItem Text="Resposta Longa" Value="1" />
                <asp:ListItem Text="Multipla Escolha" Value="2" />
                <asp:ListItem Text="Caixa De Seleção" Value="3" />
                <asp:ListItem Text="Lista Suspensa" Value="4" />
            </asp:DropDownList>
            <br />
            <br />
            <absis:Check ID="cObrigatoria" Texto="Obrigatória" runat="server"/>
        </absis:Coluna>
        <absis:TabelaComandos ID="tbcPergunta" runat="server" Style="float:left">
            <absis:Botao ID="btnAdicionarPergunta" OnClick="btnAdicionarPergunta_Click" Legenda="Adicionar" CorDoBotao="AzulEscuro" runat="server" Style="float:left"></absis:Botao>
        </absis:TabelaComandos>
    </absis:Painel>
    <absis:Mensagem ID="msgAlert" runat="server"></absis:Mensagem>
    <br />
    <br />
    <div>
        <asp:Repeater ID="rptPerguntas" runat="server" OnItemDataBound="rptPerguntas_ItemDataBound">
            <ItemTemplate>
                <div>
                    <div id="areaPergunta">
                        <b>
                            <%#Container.ItemIndex + 1 %>.
                            <br />
                            <asp:Button ID="btnEditar" runat="server" OnClick="btnEditar_Click" CommandName="editar" CommandArgument='<%#Container.ItemIndex%>'/>
                            <absis:Etiqueta ID="eTitulo" runat="server" Texto='<%# DataBinder.Eval(Container.DataItem, "Titulo") %>' EnableViewState="true"></absis:Etiqueta>
                            <asp:Label ID="lblObrigatoria" runat="server" Text="*" ForeColor="Red" Visible="false"></asp:Label>
                            <br />
                            <absis:Etiqueta ID="eDescricao" runat="server" Texto='<%# DataBinder.Eval(Container.DataItem, "Descricao") %>' EnableViewState="true"></absis:Etiqueta>
                            <br />
                        </b>
                    </div>
                    <div id="areaResposta">
                        <br />
                        <asp:RadioButtonList ID="rdRespostas" runat="server" Visible="false"></asp:RadioButtonList>
                        <asp:CheckBoxList ID="chkRespostas" runat="server" Visible="false"></asp:CheckBoxList>
                        <asp:DropDownList ID="ddlRespostas" runat="server" Visible="false" CssClass="abseed_dropdownlist"></asp:DropDownList>
                        <absis:Campo ID="txtRespostaCurta" runat="server" Visible="false"></absis:Campo>
                        <absis:Campo ID="txtRespostaLonga" runat="server" Visible="false" ModoTexto="MultiLine"></absis:Campo>
                        <br />
                        <br />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <%--<script type="text/javascript">
        $(document).delegate('#areaPergunta,#areaResposta', 'click', function () {
            var btn = $(this).find('input[type="submit"][id*="btnEditar"]');
            btn.trigger("click");
        });
    </script>--%>
</asp:Content>