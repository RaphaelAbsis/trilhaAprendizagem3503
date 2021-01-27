<%@ Page Title="" Language="C#" MasterPageFile="~/master/GenericModerna.master" AutoEventWireup="true" CodeFile="wForm_consulta.aspx.cs" Inherits="wFormulario_wForm_consulta" %>

<%@ Register Assembly="Abseed" Namespace="Abseed" TagPrefix="absis"%>
<asp:Content ID="Content1" ContentPlaceHolderID="masterBody" Runat="Server">
    <absis:PainelPesquisa ID="pnlPesquisa" runat="server" Legenda="Campos para Pesquisa" 
        EnableViewState="true" LegendaBotaoPesquisar="Pesquisar" OnPesquisar="pnlPesquisa_Pesquisar">
        <absis:Coluna ID="colEsquerda" runat="server" ColunaTipo="Esquerda" LarguraFixa="50%"> 
            <div id="Codigo">
                <absis:Etiqueta ID="lblCodigo" Texto="Código" runat="server"></absis:Etiqueta> 
                <absis:Campo ID="cpCodigo" runat="server" Width="230px">
                </absis:Campo>
            </div>
            <div id="Titulo">
               <absis:Etiqueta runat="server" ID="lblTitulo" Texto="Título"></absis:Etiqueta>
                <absis:Campo ID="cpTitulo" runat="server" Width="230px">
                </absis:Campo>
            </div>
        </absis:Coluna>
        <absis:Coluna ID="colDireita" runat="server" ColunaTipo="Direita" LarguraFixa="50%">
        </absis:Coluna>
    </absis:PainelPesquisa>
            <absis:Mensagem ID="msgConfirmaExcluir" runat="server" Tipo="Confirmacao" Texto="Confirma a exclusão dos itens selecionados ?"
        Visible="false" TextoBotaoOK="Sim" OnConfirmado="msgConfirmaExcluir_Confirmado" TextoBotaoCancelar="Não" OnCancelado="msgConfirmaExcluir_Cancelado"></absis:Mensagem>
    <absis:Mensagem runat="server" ID="msgAlert" Tipo="Alerta"></absis:Mensagem>
        <br />
    <absis:Painel ID="pnlRegistros" runat="server" Legenda="Registros" PaginadorID="pagPaginador" Visible="false">
        
    <absis:TabelaComandos ID="tbcBotoes" runat="server" Style="float: left;">
        <absis:Botao ID="btAdicionar" runat="server" OnClick="btAdicionar_Click" Legenda="Adicionar" CorDoBotao="AzulEscuro"
            Largura="150px" ></absis:Botao>
        <absis:Botao ID="btExcluir" runat="server" OnClick="btExcluir_Click" Legenda="Excluir" CorDoBotao="Vermelho"
            Largura="150px"></absis:Botao>
    </absis:TabelaComandos>
       
        <absis:Grid ID="grdRegistros" runat="server" CampoID="FormularioID" PaginadorID="pagPaginador" EnableViewState="true" OnEditarItem="grdRegistros_EditarItem">

            <Colunas>
                <absis:ColunaGrid NomeCampo="CODIGO" TextoColuna="Código" />
                <absis:ColunaGrid NomeCampo="TITULO" TextoColuna="Título" />
                <absis:ColunaGrid NomeCampo="DESCRICAO" TextoColuna="Descrição" />
            </Colunas>
        </absis:Grid>
        <absis:Paginador ID="pagPaginador" runat="server">
        </absis:Paginador>
        <br />
        <br />
    </absis:Painel>
</asp:Content>