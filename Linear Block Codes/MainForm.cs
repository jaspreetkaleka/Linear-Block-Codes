using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LinearBlockCodes
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        int n = 0;
        int k = 0;
        bool Validated_G = false;
        BitMatrix G;
        BitMatrix U;
        BitMatrix H;
        string Us;

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }
       
        #region "Validations"

        private string Validate_kn()
        {
            try
            {
                if (k_TextBox.Text == "")
                {
                    return "Error - Invalid value of 'k'";
                }

                if (n_TextBox.Text == "")
                {
                    return "Error - Invalid value of 'n'";
                }

                k = Convert.ToInt32(k_TextBox.Text);
                n = Convert.ToInt32(n_TextBox.Text);
                if (k >= n)
                {
                    return "Error - 'k' must be less than 'n'"; 
                }

                return "No Error";
            }
            catch (Exception ex)
            {
                return "Error - " + ex.Message;
            }
        }

        private string Validate_G()
        {
            try
            {
                G = null;
                U = null;
                Validated_G = false;
                G_Input_TextBox.Text = ApplyFormat(G_Input_TextBox.Text);
                G_Input_TextBox.Text = G_Input_TextBox.Text.Replace("  ", " ");

                if (G_Input_TextBox.Lines.Count() < k)
                {
                    return "Error - Number of rows of G-matrix are less than 'k'";
                }

                if (G_Input_TextBox.Lines.Count() > k)
                {
                    return "Error - Number of rows of G-matrix are greater than 'k'";
                }

                for (var i = 0; i < G_Input_TextBox.Lines.Count(); i++)
                {
                    if (G_Input_TextBox.Lines[i].Length != 2 * n)
                    {
                        return "Error - Elements of row number " + (i + 1) + " are not equal to 'n'";
                    }
                }

                if (!(new BitMatrix(G_Input_TextBox.Text.Replace(" ", "").Split(new char[] { '\n' })).SubMatrix(1, k, n - k + 1, n).IsIdentity))
                {
                    return "Error - Identity matrix not found. G-matrix must be a systematic matrix.";
                }

                Validated_G = true;
                return "No Error";
            }
            catch (Exception ex)
            {
                return "Error - " + ex.Message;
            }
        }

        #endregion

        #region "Inputs <n, k, G>

        private void kn_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue > 57 || e.KeyValue == 32)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void kn_TextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                var err = Validate_kn();
                if (err.StartsWith("Error - "))
                {
                    if (err.Contains("'k' must be less than 'n'"))
                    {
                        mainErrorProvider.SetError(k_TextBox, "k must be less than n");
                        mainErrorProvider.SetError(n_TextBox, "n must be greator than k");
                    }
                }
                else
                {
                    mainErrorProvider.Clear(); 
                    G_Input_TextBox_Leave(sender,e);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void G_Input_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue > 49)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void G_Input_TextBox_Leave(object sender, EventArgs e)
        {
            var err = Validate_G();
            if (err.StartsWith("Error - "))
            {
                mainErrorProvider.SetError(G_Input_TextBox, err);
                if (err.StartsWith("Error - Elements of row number"))
                {
                    err = err.Replace("Error - Elements of row number ", "");
                    var rowNum = Convert.ToInt32(err.Substring(0, err.IndexOf(" "))) - 1;
                    if (rowNum != G_Input_TextBox.Lines.Count() - 1)
                    {
                        G_Input_TextBox.Select(rowNum * 2 * n + rowNum, G_Input_TextBox.Text.IndexOf(Environment.NewLine, rowNum * 2 * n + rowNum + 1) - (rowNum * 2 * n + rowNum));
                    }
                    else
                    {
                        G_Input_TextBox.Select(rowNum * 2 * n + rowNum + 1, G_Input_TextBox.Text.Length - (rowNum * 2 * n + rowNum));
                    }
                }
            }
            else
            {
                mainErrorProvider.Clear();
            }
        }

        private void loadFromFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                var o = new OpenFileDialog();
                o.Filter = "Text Files|*.txt";
                o.Multiselect = false;
                o.Title = "Select Generator Matrix 'G' File.";
                if (DialogResult.OK == o.ShowDialog())
                {
                    G_Input_TextBox.Clear();
                    var rowCount = 0;
                    var colCount = 0;
                    foreach (var line in File.ReadAllLines(o.FileName))
                    {
                        var row = line.Replace(" ", "");
                        if (row != "")
                        {
                        reStart:
                            foreach (var c in row)
                            {
                                if (c != '1' && c != '0')
                                {
                                    row = row.Replace(c.ToString(), "");
                                    goto reStart;
                                }
                            }
                            if (colCount == 0)
                            {
                                colCount = row.Length;
                            }
                            else if(colCount != row.Length)
                            {
                                mainErrorProvider.SetError(G_Input_TextBox, "Number of elements in each row must be equal.");
                            }
                            G_Input_TextBox.Text = G_Input_TextBox.Text + row + Environment.NewLine; 
                            rowCount++;
                        }
                    }
                    k = rowCount;
                    n = colCount;
                    k_TextBox.Text = k.ToString();
                    n_TextBox.Text = n.ToString();
                    G_Input_TextBox_Leave(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message, "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region "Generator Matrix<G>"
        
        private void GPH_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Name.StartsWith("G_"))
            {
                GPH_TextBox.Text = ApplyFormat(G.GetString());
                GPH_TextBox.SelectAll();
                GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                GPH_TextBox.DeselectAll();
            }
            else if ((sender as RadioButton).Name.StartsWith("H_"))
            {
                GPH_TextBox.Text = ApplyFormat(H.GetString());
                GPH_TextBox.SelectAll();
                GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                GPH_TextBox.DeselectAll();
            }
            else if ((sender as RadioButton).Name.StartsWith("HT_"))
            {
                GPH_TextBox.Text = ApplyFormat(H.Transpose().GetString());
                GPH_TextBox.SelectAll();
                GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                GPH_TextBox.DeselectAll();
            }
            else if ((sender as RadioButton).Name.StartsWith("P_"))
            {
                GPH_TextBox.Text = ApplyFormat(G.SubMatrix(1, k, 1, n - k).GetString());
                GPH_TextBox.SelectAll();
                GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                GPH_TextBox.DeselectAll();
            }
            else if ((sender as RadioButton).Name.StartsWith("PT_"))
            {
                GPH_TextBox.Text = ApplyFormat(G.SubMatrix(1, k, 1, n - k).Transpose().GetString());
                GPH_TextBox.SelectAll();
                GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                GPH_TextBox.DeselectAll();
            }

            //BitMatrix PT = ;

        }

        #endregion

        #region "Error Detection"

        private void detectErrorButton_Click(object sender, EventArgs e)
        {
            r_ED_TextBox.Text = r_ED_TextBox.Text.Replace(" ", "");
            if (r_ED_TextBox.Text.Length > n)
            {
                mainErrorProvider.SetError(r_ED_TextBox, "Length of the received Vector <r> is greator than the length of the codeword.");
                return;
            }
            else if (r_ED_TextBox.Text.Length < n)
            {
                mainErrorProvider.SetError(r_ED_TextBox, "Length of the received Vector <r> is less than the length of the codeword.");
                return;
            }
            else
            {
                mainErrorProvider.Clear();
            }

            var r = new BitMatrix(new string[] { r_ED_TextBox.Text });
            var S = r.Multiply(H.Transpose());
            S_ED_TextBox.Text = S.GetString();
            if (S.IsZeroMatrix)
            {
                S_ED_Label.Text = "No Error. The received vector <r> is a valid codeword.";
                S_ED_Label.ForeColor = Color.Green;
            }
            else
            {
                S_ED_Label.Text = "Error. The received vector <r> is not a valid codeword.";
                S_ED_Label.ForeColor = Color.Red;
            }
        }

        private void r_ED_TextBox_Leave(object sender, EventArgs e)
        {
            detectErrorButton.PerformClick();
        }

        private void r_ED_TextBox_TextChanged(object sender, EventArgs e)
        {
            S_ED_TextBox.Clear();
            S_ED_Label.Text = "";
        }

        #endregion

        #region "Error Correction"

        private void correctErrorButton_Click(object sender, EventArgs e)
        {
            r_EC_TextBox.Text = r_EC_TextBox.Text.Replace(" ", "");
            if (r_EC_TextBox.Text.Length > n)
            {
                mainErrorProvider.SetError(r_EC_TextBox, "Length of the received Vector <r> is greater than the length of the codeword.");
                return;
            }
            else if (r_EC_TextBox.Text.Length < n)
            {
                mainErrorProvider.SetError(r_EC_TextBox, "Length of the received Vector <r> is less than the length of the codeword.");
                return;
            }
            else
            {
                mainErrorProvider.Clear();
            }

            var r = new BitMatrix(new string[] { r_EC_TextBox.Text });
            var S = r.Multiply(H.Transpose());
            S_EC_TextBox.Text = S.GetString();

            if (S.IsZeroMatrix)
            {
                U_EC_TextBox.Text = r_EC_TextBox.Text;
            }
            else
            {
                U_EC_TextBox.Text = CorrectError(r_EC_TextBox.Text);
            }
        }

        public string CorrectError(string r)
        {
            var totalErrors = Convert.ToInt32(Math.Pow(2, n - k)) - 1;

            var errorPatterns = new List<string>();
            errorPatterns.AddRange(GenerateErrorPattern(n, 1));
            if (errorPatterns.Count < totalErrors)
            {
                errorPatterns.AddRange(GenerateErrorPattern(n, 2));
                if (errorPatterns.Count < totalErrors)
                {
                    errorPatterns.AddRange(GenerateErrorPattern(n, 3));
                }
            }

            for (var i = 0; i < totalErrors; i++)
            {
                var e = new BitMatrix(new string[] { errorPatterns[i] });
                var S = e.Multiply(H.Transpose());
                if (S_EC_TextBox.Text == S.GetString())
                {
                    var rVct = new BitMatrix(new string[] { r });
                    return rVct.AddWithoutCarry(e).GetString();
                }
            }

            //string mBinary = "1".PadLeft(n, '0');
                            
            return "Not able to correct the error.";
        }

        public string[] GenerateErrorPattern(int patternBits, int errorBits)
        {
            var e = new List<string>();

            var block = "".PadLeft(errorBits, '1');

            var equivalentOne = new List<string>();

            for (var i = 1; i <= patternBits - errorBits; i++)
            {
                equivalentOne.AddRange(PadZeros(i + 1, "1"));
            }

            var blocks = new List<string>();

            foreach (var oneEq in equivalentOne)
            {
                for (var i = 1; i <= errorBits; i++)
                {
                    var blk = "";
                    for (var j = 0; j < i; j++)
                    {
                        blk = blk + oneEq;
                    }
                    blocks.AddRange(PadOnes(errorBits + blk.Length - i, blk));
                }
                for (var i = 1; i <= errorBits; i++)
                {
                    var blk = "";
                    for (var j = 0; j < i; j++)
                    {
                        blk = blk + "1" + oneEq;
                    }
                    var width = errorBits + blk.Length - 2 * i;
                    if (width <= patternBits)
                    {
                        if (blk.Replace("0", "").Length <= errorBits)
                        {
                            blocks.AddRange(PadOnes(width, blk));
                        }
                    }
                }
            }

            var trimmedBlocks = new List<string>();

            foreach (var blk in blocks)
            {
                var b = blk.Trim(new char[] { '0' });
                if (b.Length <= patternBits)
                {
                    if (!trimmedBlocks.Contains(b))
                    {
                        trimmedBlocks.Add(b);
                    }
                }
            }

            foreach (var blk in trimmedBlocks)
            {
                e.AddRange(PadZeros(patternBits, blk));
            }
            return e.ToArray();
        }

        public List<string> PadZeros(int totalWidth, string block)
        {
            var e = new List<string>();
            var i = 0;
            while (true)
            {
                var err = block.PadLeft(totalWidth - i, '0').PadRight(totalWidth, '0');
                e.Add(err);
                i++;
                if (err.StartsWith(block))
                {
                    break;
                }
            }
            return e;
        }

        public List<string> PadOnes(int totalWidth, string block)
        {
            var e = new List<string>();
            var i = 0;
            while (true)
            {
                var err = block.PadLeft(totalWidth - i, '1').PadRight(totalWidth, '1');
                e.Add(err);
                i++;
                if (err.StartsWith(block))
                {
                    break;
                }
            }
            return e;
        }

        private void r_EC_TextBox_Leave(object sender, EventArgs e)
        {
            correctErrorButton.PerformClick();
        }

        private void r_EC_TextBox_TextChanged(object sender, EventArgs e)
        {
            S_EC_TextBox.Clear();
            U_EC_TextBox.Clear();
        }

        #endregion

        private void mainTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                if (Validated_G)
                {
                    if (G == null)
                    {
                        G = new BitMatrix(G_Input_TextBox.Text.Replace(" ", "").Split(new char[] { '\n' }));
                        H = BitMatrix.GetIdentity(n - k).JoinColumns(G.SubMatrix(1, k, 1, n - k).Transpose());
                    }
                    GPH_TextBox.Text = ApplyFormat(G.GetString());
                    GPH_TextBox.SelectAll();
                    GPH_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                    GPH_TextBox.DeselectAll();
                }
                else
                {
                    mainTabControl.SelectedIndex = 0;
                    MessageBox.Show("Enter valid inputs for 'k', 'n' and 'G' first.", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.TabPageIndex == 2)
            {
                if (Validated_G)
                {
                    if (G == null)
                    {
                        G = new BitMatrix(G_Input_TextBox.Text.Replace(" ", "").Split(new char[] { '\n' }));
                        H = BitMatrix.GetIdentity(n - k).JoinColumns(G.SubMatrix(1, k, 1, n - k).Transpose());
                    }
                    if (U == null)
                    {
                        var r = Convert.ToInt32(Math.Pow(2, k));
                        U = new BitMatrix(r, n);
                        var sb = new StringBuilder();
                        for (var m = 0; m < r; m++)
                        {
                            var mBinary = Convert.ToString(m, 2).PadLeft(k, '0');
                            sb.Append(Environment.NewLine + mBinary + "—————— ");
                            for (var i = 0; i < mBinary.Length; i++)
                            {
                                if (mBinary[i] == '1')
                                {
                                    for (var j = 0; j < n; j++)
                                    {
                                        U[m, j] = U[m, j] + G[i][j];
                                        if (U[m][j] == 2)
                                        {
                                            U[m, j] = 0;
                                        }
                                    }
                                }
                            }
                            for (var i = 0; i < n; i++)
                            {
                                sb.Append(U[m][i].ToString());
                            }
                        }
                        var U_HT = U.Multiply(H.Transpose());
                        if (U_HT.IsZeroMatrix)
                        {
                            Us = sb.ToString();
                        }
                        else
                        {
                            Us = "Error - G matrix is not valid as U.H' not equal to zero.";
                        }
                    }
                    U_TextBox.Text = ApplyFormat(Us);
                    U_TextBox.SelectAll();
                    U_TextBox.SelectionAlignment = HorizontalAlignment.Center;
                    U_TextBox.DeselectAll();
                }
                else
                {
                    mainTabControl.SelectedIndex = 0;
                    MessageBox.Show("Enter valid inputs for 'k', 'n' and 'G' first.", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.TabPageIndex == 3 || e.TabPageIndex == 4)
            {
                if (Validated_G)
                {
                    if (G == null)
                    {
                        G = new BitMatrix(G_Input_TextBox.Text.Replace(" ", "").Split(new char[] { '\n' }));
                        H = BitMatrix.GetIdentity(n - k).JoinColumns(G.SubMatrix(1, k, 1, n - k).Transpose());
                    }
                }
                else
                {
                    mainTabControl.SelectedIndex = 0;
                    MessageBox.Show("Enter valid inputs for 'k', 'n' and 'G' first.", "Error!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public string ApplyFormat(string s)
        {
            s = s.Replace(" ", "");
            s = s.Trim(new char[] { '\n', '\r' });
            s = s.Replace("1", "1  ");
            s = s.Replace("0", "0  ");
            s = s.Replace("—", "—  ");
            return s;
        }
        
    }
}
