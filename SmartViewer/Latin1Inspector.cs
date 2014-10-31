using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Fiddler;

[assembly: RequiredVersion("4.4.9.0")]

namespace SmartViewer
{
    public class Latin1Inspector : Inspector2, IResponseInspector2
    {
        static readonly Encoding BodyEncoding = Encoding.GetEncoding(1252);
        byte[] _body;
        HTTPResponseHeaders _headers;
        TextBox _textBox;

        #region IResponseInspector2 Members

        public void Clear()
        {
            headers = null;
            body = null;
            if (null != _textBox)
                _textBox.Clear();
        }

        public byte[] body
        {
            get { return null; }
            set
            {
                _body = value;
                Refresh();
            }
        }

        public bool bDirty
        {
            get { return false; }
            set { }
        }

        public bool bReadOnly
        {
            get { return true; }
            set { }
        }

        public HTTPResponseHeaders headers
        {
            get { return null; }
            set { _headers = value; }
        }

        #endregion

        void Refresh()
        {
            if (null == _textBox)
                return;

            using (var sw = new StringWriter())
            {
                if (null != _headers)
                {
                    sw.WriteLine(_headers.HTTPVersion + " " + _headers.HTTPResponseStatus);

                    foreach (var header in _headers)
                        sw.WriteLine(header.ToString());

                    sw.WriteLine();
                }

                if (null != _body)
                {
                    using (var ms = new MemoryStream(_body, 0, _body.Length, false, false))
                    using (var sr = new StreamReader(ms, BodyEncoding))
                    {
                        for (; ; )
                        {
                            var line = sr.ReadLine();

                            if (null == line)
                                break;

                            foreach (var c in line)
                            {
                                if (0 != c)
                                    sw.Write(c);
                            }

                            sw.WriteLine();
                        }
                    }
                }

                _textBox.Text = sw.ToString();
            }
        }

        public override void AddToTab(TabPage o)
        {
            _textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Lucida Console", CONFIG.flFontSize),
                WordWrap = false,
                ScrollBars = ScrollBars.Both,
                BackColor = CONFIG.colorDisabledEdit
            };

            o.Text = "Latin1";
            o.Controls.Add(_textBox);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        public override int GetOrder()
        {
            return 0;
        }
    }
}
