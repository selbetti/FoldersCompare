using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoldersCompare
{
    public partial class frmMain : Form
    {
        int countArquivosFaltantesDireita = 0;
        int countArquivosFaltantesEsquerda = 0;
        List<string> diferentFiles = new List<string>();

        public frmMain()
        {
            InitializeComponent();
        }

        #region Eventos


        private void frmMain_Load(object sender, EventArgs e)
        {
            //limpa campos
            lblArquivosFaltantesEsquerdaDireita.Text = "";
            lblArquivosFaltantesDireitaEsquerda.Text = "";
            pnEsquerda.Visible = false;
            pnDireita.Visible = false;
            btnListarPelaDireita.Visible = false;
            btnListarPelaEsquerda.Visible = false;
        }

        /// <summary>
        /// Evento botão seleciona Path na esquerda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEsquerda_Click(object sender, EventArgs e)
        {
            

            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                if (folderBrowserDialog1.SelectedPath != null)
                {
                    txtPathEsquerda.Text = folderBrowserDialog1.SelectedPath;
                }
            }
        }

        /// <summary>
        /// Evento botão seleciona Path na direita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDireita_Click(object sender, EventArgs e)
        {
            

            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                if (folderBrowserDialog1.SelectedPath != null)
                {
                    txtPathDireira.Text = folderBrowserDialog1.SelectedPath;
                }
            }            
        }

        /// <summary>
        /// Evento botão comparar da esquerda para direita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareEsquerdaDireita_Click(object sender, EventArgs e)
        {
            //limpa campos
            lblArquivosFaltantesEsquerdaDireita.Text = "";
            lblArquivosFaltantesDireitaEsquerda.Text = "";
            countArquivosFaltantesDireita = 0;
            countArquivosFaltantesEsquerda = 0;
            pnEsquerda.Visible = false;
            pnDireita.Visible = false;
            btnListarPelaDireita.Visible = false;
            btnListarPelaEsquerda.Visible = false;
            diferentFiles.Clear();

            CompareEsquerdaDireita(txtPathEsquerda.Text, txtPathDireira.Text);
        }

        /// <summary>
        /// Evento botão comparar da direita para esquerda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareDireitaEsquerda_Click(object sender, EventArgs e)
        {
            //limpa campos
            lblArquivosFaltantesEsquerdaDireita.Text = "";
            lblArquivosFaltantesDireitaEsquerda.Text = "";
            countArquivosFaltantesDireita = 0;
            countArquivosFaltantesEsquerda = 0;
            pnEsquerda.Visible = false;
            pnDireita.Visible = false;
            btnListarPelaDireita.Visible = false;
            btnListarPelaEsquerda.Visible = false;
            diferentFiles.Clear();

            CompareDireitaEsquerda(txtPathEsquerda.Text, txtPathDireira.Text);
        }

        /// <summary>
        /// Evento tecla pressionada no listview esquerda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewEsquerda_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != listViewEsquerda) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboardEsquerda();
        }

        /// <summary>
        /// Evento tecla pressionada no listview direita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewDireita_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender != listViewDireita) return;

            if (e.Control && e.KeyCode == Keys.C)
                CopySelectedValuesToClipboardDireita();
        }

        /// <summary>
        /// Evento botão listar arquivos diferentes pela esquerda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListarPelaEsquerda_Click(object sender, EventArgs e)
        {
            frmListaFilesDirent frmFilesDiferents = new frmListaFilesDirent(diferentFiles);
            frmFilesDiferents.ShowDialog();
        }

        /// <summary>
        /// Evento botão listar arquivos diferentes pela direita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListarPelaDireita_Click(object sender, EventArgs e)
        {
            frmListaFilesDirent frmFilesDiferents = new frmListaFilesDirent(diferentFiles);
            frmFilesDiferents.ShowDialog();
        }


        #endregion

        #region Métodos

        /// <summary>
        /// Método preencher o listView da esquerda
        /// </summary>
        /// <param name="pathEsquerda">localizacao dos arquivos da esquerda</param>
        private void PreencherListViewEsquerda(string pathEsquerda)
        {
            string[] filePathsEsquerda;

            //verifica se o diretorio da esquerda existe
            if (Directory.Exists(pathEsquerda))
            {

                //pega a lista de arquivos e pastas
                filePathsEsquerda = Directory.GetFiles(pathEsquerda, "*.*", SearchOption.AllDirectories);
                Array.Sort(filePathsEsquerda);

                //informa a quantidade de arquivos
                lblQuantidadeItensEsquerda.Text = filePathsEsquerda.Count().ToString() + " arquivos listados.";

                //adiciona uma coluna na listview
                listViewEsquerda.View = View.Details;
                listViewEsquerda.Columns.Add("Arquivo", 570, HorizontalAlignment.Left);
                listViewEsquerda.Columns.Add("Tamanho", 70, HorizontalAlignment.Left);

                //acumulador do tamanho do diretorio
                long tamanhoDiretorio = 0;

                //percorre cada item e carrega no listbox
                foreach (string fileEsquerda in filePathsEsquerda)
                {
                    FileInfo fileInfo = new FileInfo(fileEsquerda);
                    string file = fileEsquerda.Replace(pathEsquerda, "");
                    ListViewItem item = new ListViewItem();
                    item.Text = file;
                    var tamanho = ConvertToKilobytes(fileInfo.Length);
                    if(tamanho > 1000)
                    {
                        tamanho = ConvertToMegaBytes(fileInfo.Length);
                        item.SubItems.Add(tamanho.ToString("0.0") + " MB");
                    }                        
                    else
                    {
                        item.SubItems.Add(tamanho.ToString("0.0") + " KB");
                    }
                        
                    listViewEsquerda.Items.Add(item);
                    tamanhoDiretorio += fileInfo.Length;                    
                }

                //apresenta o tamanho do diretorio
                lblTamanhoDiretorioEsquerda.Text = "Tamanho " + ConvertToMegaBytes(tamanhoDiretorio).ToString("0.00") + " MB";
            }
        }

        /// <summary>
        /// Método preencher o listView da direita
        /// </summary>
        /// <param name="pathDireita">localizacao dos arquivos da direita</param>
        private void PreencherListViewDireita(string pathDireita)
        {
            string[] filePathsDireita;

            //verifica se o diretorio da direita existe
            if (Directory.Exists(pathDireita))
            {
                //pega a lista de arquivos e pastas
                filePathsDireita = Directory.GetFiles(pathDireita, "*.*", SearchOption.AllDirectories);
                Array.Sort(filePathsDireita);

                //informa a quantidade de arquivos
                lblQuantidadeItensDireita.Text = filePathsDireita.Count().ToString() + " arquivos listados.";

                //adiciona uma coluna na listview
                listViewDireita.View = View.Details;
                listViewDireita.Columns.Add("Arquivo", 570, HorizontalAlignment.Left);
                listViewDireita.Columns.Add("Tamanho", 70, HorizontalAlignment.Left);

                //acumulador do tamanho do diretorio
                long tamanhoDiretorio = 0;

                //percorre cada item e carrega no listbox
                foreach (string fileDireita in filePathsDireita)
                {
                    FileInfo fileInfo = new FileInfo(fileDireita);
                    string file = fileDireita.Replace(pathDireita, "");
                    ListViewItem item = new ListViewItem();
                    item.Text = file;
                    var tamanho = ConvertToKilobytes(fileInfo.Length);
                    if (tamanho > 1000)
                    {
                        tamanho = ConvertToMegaBytes(fileInfo.Length);
                        item.SubItems.Add(tamanho.ToString("0.0") + " MB");
                    }
                    else
                    {
                        item.SubItems.Add(tamanho.ToString("0.0") + " KB");
                    }
                    listViewDireita.Items.Add(item);
                    tamanhoDiretorio += fileInfo.Length;
                }

                //apresenta o tamanho do diretorio
                lblTamanhoDiretorioDireita.Text = "Tamanho " + ConvertToMegaBytes(tamanhoDiretorio).ToString("0.00") + " MB";
            }
        }

        /// <summary>
        /// Método compara da esquerda para direita
        /// </summary>
        /// <param name="pathEsquerda">localizacao dos arquivos da esquerda</param>
        /// <param name="pathDireita">localizacao dos arquivos da direita</param>
        private void CompareEsquerdaDireita(string pathEsquerda, string pathDireita)
        {  
            listViewEsquerda.Clear();
            listViewDireita.Clear();

            PreencherListViewEsquerda(pathEsquerda);
            PreencherListViewDireita(pathDireita);

            //Comparação da esquerda para direita
            for (int i = 0; i < listViewEsquerda.Items.Count - 1; i++)
            {
                FileInfo fileEsquerda = new FileInfo(listViewEsquerda.Items[i].Text);
                bool contem = false;

                foreach (ListViewItem item in listViewDireita.Items)
                {
                    FileInfo fileDireita = new FileInfo(item.Text);

                    if (String.Equals(fileEsquerda.Name, fileDireita.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        contem = true;
                        break;
                    }
                }

                if (!contem)
                {
                    listViewEsquerda.Items[i].BackColor = Color.GreenYellow;
                    diferentFiles.Add(listViewEsquerda.Items[i].Text);
                    countArquivosFaltantesDireita ++;
                }
            }

            lblArquivosFaltantesEsquerdaDireita.Text = "Existem " + countArquivosFaltantesDireita + " arquivos a mais do que o lado direito.";

            //apresenta os paineis com resultados
            pnEsquerda.Visible = true;
            pnDireita.Visible = true;            
            btnListarPelaEsquerda.Visible = true;
        }

        /// <summary>
        /// Método compara da direita para esquerda
        /// </summary>
        /// <param name="pathEsquerda">localizacao dos arquivos da esquerda</param>
        /// <param name="pathDireita">localizacao dos arquivos da direita</param>
        private void CompareDireitaEsquerda(string pathEsquerda, string pathDireita)
        {
            listViewEsquerda.Clear();
            listViewDireita.Clear();

            PreencherListViewEsquerda(pathEsquerda);

            PreencherListViewDireita(pathDireita);


            //Comparação da esquerda para direita
            for (int i = 0; i < listViewDireita.Items.Count - 1; i++)
            {
                FileInfo fileEsquerda = new FileInfo(listViewDireita.Items[i].Text);
                bool contem = false;

                foreach (ListViewItem item in listViewEsquerda.Items)
                {
                    FileInfo fileDireita = new FileInfo(item.Text);

                    if (String.Equals(fileDireita.Name, fileEsquerda.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        contem = true;
                        break;
                    }
                }

                if (!contem)
                {
                    listViewDireita.Items[i].BackColor = Color.GreenYellow;
                    countArquivosFaltantesEsquerda++;
                    diferentFiles.Add(listViewDireita.Items[i].Text);
                }
            }

            lblArquivosFaltantesDireitaEsquerda.Text = "Existem " + countArquivosFaltantesEsquerda + " arquivos a mais do que o lado esquerdo.";

            //apresenta os paineis com resultados
            pnEsquerda.Visible = true;
            pnDireita.Visible = true;
            btnListarPelaDireita.Visible = true;            
        }
        
        /// <summary>
        /// Método converte valores para Kilobytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private double ConvertToKilobytes(long bytes)
        {
            return bytes / 1024f;
        }

        /// <summary>
        /// Método converte valores para MegaBytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private double ConvertToMegaBytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        /// <summary>
        /// Método copia do listview esquerda para o ClipBoard
        /// </summary>
        private void CopySelectedValuesToClipboardEsquerda()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in listViewEsquerda.SelectedItems)
                builder.AppendLine(item.Text);

            Clipboard.SetText(builder.ToString());
        }

        /// <summary>
        /// Método copia do listview direita para o ClipBoard
        /// </summary>
        private void CopySelectedValuesToClipboardDireita()
        {
            var builder = new StringBuilder();
            foreach (ListViewItem item in listViewDireita.SelectedItems)
                builder.AppendLine(item.Text);

            Clipboard.SetText(builder.ToString());
        }





        #endregion

       
    }
}
