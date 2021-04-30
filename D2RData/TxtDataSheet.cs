using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace D2Data
{
    /// <summary>
    /// Classification to represent a tab-splitted txt-based 2D data sheet. (With/without quote signs)
    /// </summary>
    public class TxtDataSheet
    {
        #region Fields

        private List<string> _headerRow = null;
        private List<string> _headerColumn = null;
        private List<string[]> _data = null;
        private bool _bHasHeaderRow = false;
        private bool _bHasHeaderColumn = false;
        private int _iColumnCount = 0;
        private string _sFilename = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of TxtDataSheet class from specified text file with header 
        /// line and ignore all malformed lines except the header line.
        /// </summary>
        /// <param name="filename">Text file path.</param>
        public TxtDataSheet(string filename)
            : this(filename, true, true, true, true)
        {
        }


        /// <summary>
        /// Creates an instance of TxtDataSheet class from specified text file.
        /// </summary>
        /// <param name="filename">Text file path.</param>
        /// <param name="hasHeaderRow">Indicates whether this sheet has a header row.</param>
        /// <param name="ignoreMalformedLine">Indicates whether ignore malformed line.</param>
        /// <param name="hasHeaderColumn">Indicates whether this sheet has a header column.</param>
        /// <param name="trimQuote">Indicates whether trim leading and tailing quotes.</param>
        public TxtDataSheet(string filename, bool hasHeaderRow, bool hasHeaderColumn, bool ignoreMalformedLine, bool trimQuote)
            : this(filename, Encoding.Default, hasHeaderRow, hasHeaderColumn, ignoreMalformedLine, trimQuote)
        {
        }

        /// <summary>
        /// Creates an instance of TxtDataSheet class from specified text file.
        /// </summary>
        /// <param name="filename">Text file path.</param>
        /// <param name="encoding">Encoding of the text file.</param>
        /// <param name="hasHeaderRow">Indicates whether this sheet has a header row.</param>
        /// <param name="ignoreMalformedLine">Indicates whether ignore malformed line.</param>
        /// <param name="hasHeaderColumn">Indicates whether this sheet has a header column.</param>
        /// <param name="trimQuote">Indicates whether trim leading and tailing quotes.</param>
        public TxtDataSheet(string filename, Encoding encoding, bool hasHeaderRow, bool hasHeaderColumn, bool ignoreMalformedLine, bool trimQuote)
        {
            FileStream fs = null;
            StreamReader sr = null;

            try
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException();
                }

                fs = new FileStream(filename, FileMode.Open);
                sr = new StreamReader(fs, encoding);

                _sFilename = filename;

                if (hasHeaderRow)
                {
                    string header = sr.ReadLine();
                    if (string.IsNullOrEmpty(header))
                    {
                        throw new EndOfStreamException("There is no header row.");
                    }
                    string[] data = header.Split('\t');
                    if (data != null && data.Length > 0)
                    {
                        _iColumnCount = data.Length;
                        _headerRow = new List<string>(data.Length);
                        foreach (string tab in data)
                        {
                            if (trimQuote)
                            {
                                _headerRow.Add(TrimQuote(tab));
                            }
                        }
                    }
                    else
                    {
                        throw new EndOfStreamException("Header row contains no data.");
                    }
                    _bHasHeaderRow = true;
                }

                string line = sr.ReadLine();
                _data = new List<string[]>();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] data = line.Split('\t');
                        if (data != null && data.Length > 0)
                        {
                            if (_iColumnCount == 0)
                            {
                                _iColumnCount = data.Length;
                            }

                            if (!ignoreMalformedLine && _iColumnCount != data.Length)
                            {
                                throw new InvalidDataException("Malformed line data.");
                            }
                            else
                            {
                                if (trimQuote)
                                {
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        data[i] = TrimQuote(data[i]);
                                    }
                                }
                                _data.Add(data);
                            }
                        }
                    }
                    line = sr.ReadLine();
                }

                if (hasHeaderColumn)
                {
                    _headerColumn = new List<string>();
                    foreach (string[] lineData in _data)
                    {
                        _headerColumn.Add(lineData[0]);
                    }
                    _bHasHeaderColumn = true;
                }
                
            }
            finally
            {

            }
        }

        /// <summary>
        /// Creates an instance of TxtDataSheet class from specified text file.
        /// </summary>
        /// <param name="filestream">Text file stream.</param>
        /// <param name="hasHeaderRow">Indicates whether this sheet has a header row.</param>
        /// <param name="ignoreMalformedLine">Indicates whether ignore malformed line.</param>
        /// <param name="hasHeaderColumn">Indicates whether this sheet has a header column.</param>
        /// <param name="trimQuote">Indicates whether trim leading and tailing quotes.</param>
        /// <param name="filename">Filename of text file.</param>
        public TxtDataSheet(Stream filestream, string filename, bool hasHeaderRow, bool hasHeaderColumn, bool ignoreMalformedLine, bool trimQuote)
            : this(filestream, Encoding.Default, filename, hasHeaderRow, hasHeaderColumn, ignoreMalformedLine, trimQuote)
        {
        }

        /// <summary>
        /// Creates an instance of TxtDataSheet class from specified text file.
        /// </summary>
        /// <param name="filestream">Text file stream.</param>
        /// <param name="encoding">Encoding of the stream.</param>
        /// <param name="hasHeaderRow">Indicates whether this sheet has a header row.</param>
        /// <param name="ignoreMalformedLine">Indicates whether ignore malformed line.</param>
        /// <param name="hasHeaderColumn">Indicates whether this sheet has a header column.</param>
        /// <param name="trimQuote">Indicates whether trim leading and tailing quotes.</param>
        /// <param name="filename">Filename of text file.</param>
        public TxtDataSheet(Stream filestream, Encoding encoding, string filename, bool hasHeaderRow, bool hasHeaderColumn, bool ignoreMalformedLine, bool trimQuote)
        {
            StreamReader sr = null;

            try
            {
                filestream.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(filestream, encoding);

                _sFilename = filename;

                if (hasHeaderRow)
                {
                    string header = sr.ReadLine();
                    if (string.IsNullOrEmpty(header))
                    {
                        throw new EndOfStreamException("There is no header row.");
                    }
                    string[] data = header.Split('\t');
                    if (data != null && data.Length > 0)
                    {
                        _iColumnCount = data.Length;
                        _headerRow = new List<string>(data.Length);
                        foreach (string tab in data)
                        {
                            if (trimQuote)
                            {
                                _headerRow.Add(TrimQuote(tab));
                            }
                        }
                    }
                    else
                    {
                        throw new EndOfStreamException("Header row contains no data.");
                    }
                    _bHasHeaderRow = true;
                }

                string line = sr.ReadLine();
                _data = new List<string[]>();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] data = line.Split('\t');
                        if (data != null && data.Length > 0)
                        {
                            if (_iColumnCount == 0)
                            {
                                _iColumnCount = data.Length;
                            }

                            if (!ignoreMalformedLine && _iColumnCount != data.Length)
                            {
                                throw new InvalidDataException("Malformed line data.");
                            }
                            else
                            {
                                if (trimQuote)
                                {
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        data[i] = TrimQuote(data[i]);
                                    }
                                }
                                _data.Add(data);
                            }
                        }
                    }
                    line = sr.ReadLine();
                }

                if (hasHeaderColumn)
                {
                    _headerColumn = new List<string>();
                    foreach (string[] lineData in _data)
                    {
                        _headerColumn.Add(lineData[0]);
                    }
                    _bHasHeaderColumn = true;
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets data in specified cell.
        /// </summary>
        /// <param name="rowIndex">Zero-based index of row. (Excluding the header row if has)</param>
        /// <param name="columnIndex">Zero-based index of column. (Including the header column if has)</param>
        /// <returns>Data in specified cell.</returns>
        public string this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex >= _data.Count || columnIndex >= _iColumnCount)
                {
                    throw new IndexOutOfRangeException();
                }

                return _data[rowIndex][columnIndex];
            }
            set
            {
                if (rowIndex >= _data.Count || columnIndex >= _iColumnCount)
                {
                    throw new IndexOutOfRangeException();
                }

                _data[rowIndex][columnIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets data in specified cell.
        /// </summary>
        /// <param name="rowKey">Key of row.</param>
        /// <param name="columnKey">Key of column.</param>
        /// <returns>Data in specified cell.</returns>
        public string this[string rowKey, string columnKey]
        {
            get
            {
                if (!_bHasHeaderRow || !_bHasHeaderColumn)
                {
                    throw new InvalidOperationException("This text data sheet does not contain a header row or a header column.");
                }

                int y = _headerColumn.IndexOf(rowKey);
                int x = _headerRow.IndexOf(columnKey);
                
                return this[y, x];
            }
            set
            {
                if (!_bHasHeaderRow || !_bHasHeaderColumn)
                {
                    throw new InvalidOperationException("This text data sheet does not contain a header row or a header column.");
                }

                int y = _headerColumn.IndexOf(rowKey);
                int x = _headerRow.IndexOf(columnKey);

                this[y, x] = value;
            }
        }

        /// <summary>
        /// Gets or sets data in specified cell.
        /// </summary>
        /// <param name="rowIndex">Index of row.</param>
        /// <param name="columnKey">Key of column.</param>
        /// <returns>Data in specified cell.</returns>
        public string this[int rowIndex, string columnKey]
        {
            get
            {
                if (!_bHasHeaderRow)
                {
                    throw new InvalidOperationException("This text data sheet does not contain a header row.");
                }

                int x = _headerRow.IndexOf(columnKey);

                return this[rowIndex, x];
            }
            set
            {
                if (!_bHasHeaderRow)
                {
                    throw new InvalidOperationException("This text data sheet does not contain a header row.");
                }

                int x = _headerRow.IndexOf(columnKey);

                this[rowIndex, x] = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the total count of rows. (Excluding the header row if has)
        /// </summary>
        public int RowCount
        {
            get
            {
                return _data.Count;
            }
        }

        /// <summary>
        /// Gets the total count of columns.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return _iColumnCount;
            }
        }

        /// <summary>
        /// Gets the header row if has.
        /// </summary>
        public List<string> Header
        {
            get
            {
                if (!_bHasHeaderRow)
                {
                    return null;
                }
                else
                {
                    return _headerRow;
                }
            }
        }

        /// <summary>
        /// Ges the text filename.
        /// </summary>
        public string Filename
        {
            get
            {
                return _sFilename;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Trim leading and tailing quotes.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <returns></returns>
        private string TrimQuote(string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length < 2)
            {
                return source;
            }

            if (source.Substring(0, 1) == "\"" && source.Substring(source.Length - 1) == "\"")
            {
                return source.Substring(1, source.Length - 2);
            }

            return source;
        }

        /// <summary>
        /// Sets data into specified header column.
        /// </summary>
        /// <param name="columnIndex">Index of column.</param>
        /// <param name="data">Data to set.</param>
        public void SetHeaderColumn(int columnIndex, string data)
        {
            if (!_bHasHeaderRow)
            {
                throw new InvalidOperationException("This data sheet does not have a header row.");
            }

            if (columnIndex >= _iColumnCount)
            {
                throw new IndexOutOfRangeException();
            }

            _headerRow[columnIndex] = data;
        }

        /// <summary>
        /// Insert an empty row with all null data before specified line.
        /// </summary>
        /// <param name="beforeLineIndex">Zero-based index of row before which the new line will be inserted.</param>
        public void InsertRowBefore(int beforeLineIndex)
        {
            if (beforeLineIndex < 0 || beforeLineIndex >= _data.Count)
            {
                throw new IndexOutOfRangeException();
            }

            _data.Insert(beforeLineIndex, new string[_iColumnCount]);
        }

        /// <summary>
        /// Insert an empty line with all null data after specified line.
        /// </summary>
        /// <param name="afterLineIndex">Zero-based index of row after which the new line will be inserted.</param>
        public void InsertRowAfter(int afterLineIndex)
        {
            if (afterLineIndex < 0 || afterLineIndex >= _data.Count)
            {
                throw new IndexOutOfRangeException();
            }

            _data.Insert(afterLineIndex + 1, new string[_iColumnCount]);
        }

        /// <summary>
        /// Appends an empty row at the end of the data sheet with all null data.
        /// </summary>
        public void AppendRow()
        {
            InsertRowAfter(_data.Count - 1);
        }

        /// <summary>
        /// Adds a header row for this data sheet.
        /// </summary>
        public void AddHeaderRow()
        {
            if (_bHasHeaderRow)
            {
                throw new InvalidOperationException("This data sheet already contains a header line.");
            }

            _headerRow = new List<string>(_iColumnCount);
            _bHasHeaderRow = true;
        }

        /// <summary>
        /// Removes the header row of this data sheet.
        /// </summary>
        public void RemoveHeaderRow()
        {
            _bHasHeaderRow = false;
            _headerRow = null;
        }

        /// <summary>
        /// Saves this data sheet into a stream.
        /// </summary>
        /// <param name="filename">Path of the target file.</param>
        /// <param name="addQuote">Adds quotes around text if it contains space.</param>
        public Stream Save(bool addQuote)
        {
            MemoryStream ms = null;
            StreamWriter sw = null;

            try
            {
                ms = new MemoryStream();
                sw = new StreamWriter(ms, Encoding.GetEncoding(1200));

                if (_bHasHeaderRow)
                {
                    for (int i = 0; i < _headerRow.Count; i++)
                    {
                        if (addQuote && _headerRow[i].Contains(' '))
                        {
                            sw.Write('"');
                        }
                        sw.Write(_headerRow[i]);
                        if (addQuote && _headerRow[i].Contains(' '))
                        {
                            sw.Write('"');
                        }
                        if (i < _headerRow.Count - 1)
                        {
                            sw.Write('\t');
                        }
                    }
                    sw.WriteLine();
                }

                foreach (string[] data in _data)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (addQuote && data[i].Contains(' '))
                        {
                            sw.Write('"');
                        }
                        sw.Write(data[i]);
                        if (addQuote && data[i].Contains(' '))
                        {
                            sw.Write('"');
                        }
                        if (i < data.Length - 1)
                        {
                            sw.Write('\t');
                        }
                    }
                    sw.WriteLine();
                }
            }
            catch
            {
            }

            return ms;
        }

        #endregion
    }
}
