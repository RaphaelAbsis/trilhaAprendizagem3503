<%@ Page Title="" Language="C#" MasterPageFile="~/master/GenericModerna.master" AutoEventWireup="true" CodeFile="wTrei_consulta.aspx.cs" Inherits="wTreinamentos_wTrei_consulta" %>

<%@ Register Assembly="Abseed" Namespace="Abseed" TagPrefix="absis" %>
<asp:Content ID="Content1" ContentPlaceHolderID="masterBody" runat="Server">
    <absis:PainelPesquisa ID="pnlPesquisa" runat="server" Legenda="Campos para Pesquisa"
        EnableViewState="true" LegendaBotaoPesquisar="Pesquisar" OnPesquisar="pnlPesquisa_Pesquisar">
        <absis:Coluna ID="colEsquerda" runat="server" ColunaTipo="Esquerda" LarguraFixa="50%">
            <absis:Campo ID="txtPesqCodigo" runat="server" Legenda="Código" EnableViewState="true">
            </absis:Campo>
            <br />
            <absis:Campo ID="txtPesqTitulo" runat="server" Legenda="Título" EnableViewState="true">
            </absis:Campo>
        </absis:Coluna>
        <absis:Coluna ID="colDireita" runat="server" ColunaTipo="Direita" LarguraFixa="50%">
            <absis:Periodo ID="txtPesqPeriodo" runat="server" Legenda="Período" SugereDataFim="false"></absis:Periodo>
        </absis:Coluna>
    </absis:PainelPesquisa>
    <absis:Painel ID="pnlRegistros" runat="server" Legenda="Registros" PaginadorID="pagPaginador">
        <absis:Mensagem runat="server" ID="msgAlert" Tipo="Alerta"></absis:Mensagem>
        <absis:Mensagem runat="server" ID="MsgConfirma" Tipo="Sucesso"></absis:Mensagem>
        <absis:Mensagem runat="server" ID="msgModalConfirmacaoErro" Tipo="Erro"></absis:Mensagem>
        <absis:Mensagem ID="msgConfirmaExcluir" runat="server" Tipo="Confirmacao" Texto="Confirma a exclusão dos itens selecionados ?"
            Visible="false" TextoBotaoOK="Sim" OnConfirmado="msgConfirmaExcluir_Confirmado" TextoBotaoCancelar="Não" OnCancelado="msgConfirmaExcluir_Cancelado"></absis:Mensagem>
        <absis:TabelaComandos ID="TabelaComandos1" runat="server" Style="float: left;">
            <absis:Botao ID="btAdicionar" runat="server" Legenda="Adicionar" CorDoBotao="AzulEscuro"
                OnClick="btAdicionar_Click" Largura="150px"></absis:Botao>
            <absis:Botao ID="btExcluir" runat="server" Legenda="Excluir" CorDoBotao="Vermelho"
                OnClick="btExcluir_Click" Largura="150px"></absis:Botao>
        </absis:TabelaComandos>
        <absis:Grid ID="grdRegistros" runat="server" CampoID="TREINAMENTOID" PaginadorID="pagPaginador" EnableViewState="true" OnEditarItem="grdRegistros_EditarItem" OnLinhaCriada="grdRegistros_LinhaCriada">
            <Colunas>
                <absis:ColunaGrid NomeCampo="TREINAMENTOID" TextoColuna="Id" />
                <absis:ColunaGrid NomeCampo="CODIGO" TextoColuna="Código" />
                <absis:ColunaGrid NomeCampo="TITULO" TextoColuna="Título" />
                <absis:ColunaGrid NomeCampo="DATAINICIAL" TextoColuna="Data Inicial"/>
                <absis:ColunaGrid NomeCampo="DATAFINAL" TextoColuna="Data Final"/>
                <absis:ColunaGrid NomeCampo="CARGAHORARIA" TextoColuna="Carga Horária" />
                <absis:ColunaGrid NomeCampo="PRAZO" TextoColuna="Prazo" />
                <absis:ColunaGrid NomeCampo="FORMULARIOID" TextoColuna="Tem Avaliação" />
                <%--<absis:ColunaGrid NomeCampo="LINKVIDEO" TextoColuna="Tem LinkVideo" />--%>
                <absis:ColunaGrid NomeCampo="INSTRUTOR" TextoColuna="Instrutor" />
                <absis:ColunaGrid NomeCampo="ORIENTADOR" TextoColuna="Orientador" />
                <%--<absis:ColunaGrid NomeCampo="CHECKLISTID" TextoColuna="Tem CheckList" />--%>
            </Colunas>
        </absis:Grid>
        <absis:Paginador ID="pagPaginador" runat="server">
        </absis:Paginador>
        <br />
        <br />
    </absis:Painel>
    <!--Modal-->
    <div id="divFundo" style="position: fixed; width: 100%; height: 100%; background-color: black; opacity: 0.75; z-index: 997; top: 0px; left: 0px;"
        runat="server" visible="false">
    </div>
    <div id="divInclusao" class="abseed_conteudo_sub" style="width: 900px; height: auto; position: absolute; top: 50%; left: 50%; margin-top: -318px; margin-left: -450px; z-index: 998; padding: 0px; border-radius: 5px;"
        runat="server" visible="false">
        <div class="abseed_conteudo_borda" style="text-align: center; color: #ffffff;">

            <absis:Etiqueta runat="server" ID="TituloModal" Texto="Adicionar Treinamento" Style="height: auto"></absis:Etiqueta>
        </div>
        <br />
        <absis:Mensagem runat="server" ID="msgAlertModal" Tipo="Alerta"></absis:Mensagem>
        <div style="padding: 10px;">
            <absis:Coluna ID="colItemEsquerda" runat="server" ColunaTipo="Esquerda" LarguraFixa="50%">
                <asp:HiddenField ID="hfId" runat="server" />
                <absis:Campo ID="txtCodigo" runat="server" Legenda="Código" PreenchimentoObrigatorio="true">
                </absis:Campo>
                <br />
                <absis:Data ID="txtDataInicial" runat="server" Legenda="Data Inicial"></absis:Data>
                <br />
                <absis:Data ID="txtDataFinal" runat="server" Legenda="Data Final"></absis:Data>
                <br />
                <absis:Campo ID="txtLinkVideo" runat="server" Legenda="Vídeo">
                </absis:Campo>
                <br />
                <absis:PessoaSeletor runat="server" ID="psInstrutor" Legenda="Instrutor"></absis:PessoaSeletor>

                
            </absis:Coluna>
            <absis:Coluna ID="colItemDireita" runat="server" ColunaTipo="Direita" LarguraFixa="50%">
                <absis:Campo ID="txtTitulo" runat="server" Legenda="Título">
                </absis:Campo>
                <br />
                <absis:Etiqueta runat="server" ID="lblFormulario" Texto="Avaliação"></absis:Etiqueta>
                <asp:DropDownList ID="dpFormulario" runat="server" CssClass="abseed_dropdownlist" style="margin-left:-3px;">
                </asp:DropDownList>
                <br />
                <absis:Campo ID="txtPrazo" runat="server" Legenda="Prazo" EnableViewState="true" Numerico="true" TamanhoMaximo="3"></absis:Campo>
                <%--<absis:Data ID="txtPrazo" runat="server" Legenda="Prazo" SugereDataFim="false"></absis:Data>--%>
                <br />
                <absis:Hora ID="txtCargaHoraria" runat="server" Legenda="Cara Horária" HoraNormal="false" QuantidadeDigitosHora="2"></absis:Hora>
                <br />
                <absis:PessoaSeletor runat="server" ID="psOrientador" Legenda="Orientador"></absis:PessoaSeletor>
                <br />
                 <absis:Etiqueta runat="server" ID="lblChecklist" Texto="Checklist"></absis:Etiqueta>

                <asp:FileUpload runat="server" ID="flpChecklist" EnableViewState="true" ViewStateMode="Enabled" />
                <asp:HiddenField ID="hdCaminho" runat="server" />
                <asp:LinkButton ID="lnkChecklist" runat="server" Text="Clique aqui para visualizar o arquivo selecionado"></asp:LinkButton>
                
                <%--<br />
                <absis:Etiqueta runat="server" ID="lblChecklist" Texto="Checklist"></absis:Etiqueta>
                <asp:FileUpload runat="server" ID="flpChecklist" EnableViewState="true" />--%>
            </absis:Coluna>
            <h3>Alvo</h3>
            <div style="margin-bottom: 15px;">
                <absis:Mensagem ID="msgAlertaAlvo" runat="server" Texto="É necessário selecionar ao menos um dos campos abaixo." Tipo="Alerta"></absis:Mensagem>
                <absis:TabelasSeletor runat="server" ID="tsCargo" Tabela="040" Legenda="Cargo"></absis:TabelasSeletor>
                <br />
                <absis:TabelasSeletor runat="server" ID="tsLotacao" Tabela="030" Legenda="Lotação"></absis:TabelasSeletor>
                <br />
                <absis:TabelasSeletor runat="server" ID="tsEstab" Tabela="080" Legenda="Estab."></absis:TabelasSeletor>
            </div>
            <absis:Botao ID="btnAddAlvo" runat="server" OnClick="btnAddAlvo_Click" Legenda="Adicionar" CorDoBotao="Verde" Width="150px" Style="margin-left: 10px;"></absis:Botao>
            <absis:Botao ID="btnExcluirAlvo" runat="server" Legenda="Excluir" CorDoBotao="Vermelho" OnClick="btnExcluirAlvo_Click" Width="150px"></absis:Botao>
            <absis:Grid runat="server" ID="grdModal" CampoID="CARGO,LOTACAO,ESTAB" ColocaLinhasEmBranco="false" ItensPorPagina="10">
                <Colunas>
                    <absis:ColunaGrid NomeCampo="CARGO" TextoColuna="Cargo" />
                    <absis:ColunaGrid NomeCampo="CARGOTIT" TextoColuna="Título" />
                    <absis:ColunaGrid NomeCampo="LOTACAO" TextoColuna="Lotação" />
                    <absis:ColunaGrid NomeCampo="LOTACAOTIT" TextoColuna="Título" />
                    <absis:ColunaGrid NomeCampo="ESTAB" TextoColuna="Estab." />
                    <absis:ColunaGrid NomeCampo="ESTABTIT" TextoColuna="Título" />
                </Colunas>
            </absis:Grid>
            <div style="float: left; padding: 10px; bottom: 0;">
                <absis:Botao ID="btnSalvar" runat="server" Legenda="Salvar" CorDoBotao="Verde" OnClick="btnSalvar_Click" Width="150px"></absis:Botao>
            </div>
            <div style="float: right; padding: 10px; bottom: 0;">
                <absis:Botao ID="btnSair" runat="server" CorDoBotao="Vermelho" Legenda="Sair" OnClick="btnSair_Click" Width="150px"></absis:Botao>
            </div>
        </div>
    </div>

    <script src="../layout/js/ajaxfileupload.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).delegate('input[type="file"][id*="flpChecklist"]', 'change', function () {
            var flpValue = $(this).val();
            var vImagem = flpValue.toString().replace('C:\\fakepath\\', '');
            if (vImagem != '') {
                $(this).parent().find('input[type="hidden"][id*="hfAnexo"]').val(vImagem.substring(vImagem.lastIndexOf('\\') + 1, vImagem.length));
                $(this).parent().find('input[type="hidden"][id*="hfAlterouAnexo"]').val("S");
                $(this).parent().parent().find('a[id*="lnkAnexo"]').show();
                ajaxFileUpload(vImagem, $(this).attr("id"));
            }
        });
        function ajaxFileUpload(vImagem, elementId) {
            $.ajaxFileUpload({
                url: '../FileUpload.ashx',
                secureuri: false,
                fileElementId: elementId,
                dataType: 'json',
                data: {
                    tipo: 'temp',
                    id: 'id'
                },
                success: function (data, status) {
                },
                error: function (data, status, e) {
                    alert(e);
                }
            });
            return false;
        }
        
       $(document).delegate('input[type="file"][id*="flpChecklist"]', 'change', function () {
            var flpValue = $(this).val();
           var vImagem = flpValue.toString().replace('C:\\fakepath\\', '');
           $('input[type="hidden"][id*=hdCaminho]').val(vImagem);
       });

        $(document).delegate('a[id*=lnkChecklist]', 'click', function () {
                   window.open('../temp/'+$('input[type="hidden"][id*=hdCaminho]').val());
                   return false;
               });
    </script>
</asp:Content>