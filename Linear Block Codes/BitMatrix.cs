using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinearBlockCodes
{
    class BitMatrix
    {
        List<List<int>> bm;
       
        public BitMatrix()
        {
            bm = new List<List<int>>();
        }
        
        public BitMatrix(string[] m)
        {
            bm = new List<List<int>>();
            foreach (var s in m)
            {
                var b = new List<int>();
                foreach (var c in s)
                {
                    if (c == '1')
                    {
                        b.Add(1);
                    }
                    else if (c == '0')
                    {
                        b.Add(0);
                    }
                }
                bm.Add(b);
            }
        }

        public BitMatrix(List<List<char>> m)
        {
            bm = new List<List<int>>();
            foreach (var s in m)
            {
                var b = new List<int>();
                foreach (var c in s)
                {
                    if (c == '1')
                    {
                        b.Add(1);
                    }
                    else if (c == '0')
                    {
                        b.Add(0);
                    }
                }
                bm.Add(b);
            }
        }

        public BitMatrix(int rows, int columns)
        {
            bm = new List<List<int>>();
            for (var i = 0; i < rows; i++)
            {
                var b = new List<int>();
                for (var j = 0; j < columns; j++)
                {
                    b.Add(0);
                }
                bm.Add(b);
            }
        }

        public static BitMatrix GetIdentity(int rc)
        {
            var temp = new BitMatrix(rc, rc);
            for (var i = 0; i < temp.bm.Count; i++)
            {
                for (var j = 0; j < temp.bm.Count; j++)
                {
                    if (i == j)
                    {
                        temp.bm[i][j] = 1;
                    }
                }
            }
            return temp;
        }

        public int[] this[int r]
        {
            get
            {
                return bm[r].ToArray();
            }
            set
            {
                bm[r] = new List<int>(value);
            }
        }

        public int this[int r, int c]
        {
            get
            {
                return bm[r][c];
            }
            set
            {
                bm[r][c] = value;
            }
        }

        public BitMatrix SubMatrix(int rowStart, int rowEnd, int colStart, int colEnd)
        {
            var sBm = new BitMatrix();
            var cols = colEnd - colStart + 1;
            for (var i = rowStart - 1; i < rowEnd; i++)
            {
                sBm.bm.Add(bm[i].GetRange(colStart - 1, cols));
            }

            return sBm;
        }

        public bool IsIdentity
        {
            get
            {
                if (bm.Count < 0)
                {
                    return false;
                }
                if (bm.Count != bm[0].Count)
                {
                    return false;
                }
                else
                {
                    for (var i = 0; i < bm.Count; i++)
                    {
                        for (var j = 0; j < bm.Count; j++)
                        {
                            if (i == j)
                            {
                                if (bm[i][j] != 1)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (bm[i][j] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
        }

        public bool IsZeroMatrix
        {
            get
            {
                for (var i = 0; i < bm.Count; i++)
                {
                    for (var j = 0; j < bm[0].Count; j++)
                    {
                        if (bm[i][j] != 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public string GetString()
        {
            var sb = new StringBuilder();
            foreach (var r in bm)
            {
                foreach (var b in r)
                {
                    sb.Append(b);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public BitMatrix JoinColumns(BitMatrix b)
        {
            var colNum = bm[0].Count + b.bm[0].Count;
            var temp = new BitMatrix(bm.Count, colNum);
            for (var i = 0; i < bm.Count; i++)
            {
                for (var j = 0; j < bm[0].Count; j++)
                {
                    temp.bm[i][j] = bm[i][j];                 
                }
                for (var j = bm[0].Count; j < colNum; j++)
                {
                    temp.bm[i][j] = b.bm[i][j - bm[0].Count];       
                }
            }
            return temp;
        }

        public BitMatrix Transpose()
        {
            var temp = new BitMatrix(bm[0].Count, bm.Count);
            for (var i = 0; i < bm.Count; i++)
            {
                for (var j = 0; j < bm[0].Count; j++)
                {
                    temp.bm[j][i] = bm[i][j];
                }
            }
            return temp;
        }

        public BitMatrix Multiply(BitMatrix b)
        {
            var temp = new BitMatrix(bm.Count, b.bm[0].Count);
            for (var i = 0; i < bm.Count; i++)
            {
                for (var j = 0; j < b.bm[0].Count(); j++)
                {
                    for (var k = 0; k < bm[0].Count; k++)
                    {
                        temp.bm[i][j] = temp.bm[i][j] + bm[i][k] * b.bm[k][j];
                    }
                    temp.bm[i][j] = temp.bm[i][j] & 1;
                }
            }
            return temp;
        }

        public BitMatrix AddWithoutCarry(BitMatrix b)
        {
            var temp = new BitMatrix(bm.Count, b.bm[0].Count);
            for (var i = 0; i < bm.Count; i++)
            {
                for (var j = 0; j < bm[0].Count(); j++)
                {
                    temp.bm[i][j] = bm[i][j] + b.bm[i][j];
                    if (temp.bm[i][j] == 2)
                    {
                        temp.bm[i][j] = 0;
                    }
                }
            }
            return temp;
        }

    }
}
