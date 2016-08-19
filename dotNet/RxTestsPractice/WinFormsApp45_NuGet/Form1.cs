using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PortableLibraryProfile78_NuGet;

namespace WinFormsApp45_NuGet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var clock = MyExtensions.GetClock();

            var input = 
                    Observable.FromEventPattern(textBox1, "TextChanged").
                        Select(evt => ((TextBox)evt.Sender).Text).Throttle(TimeSpan.FromSeconds(.2)).
                        DistinctUntilChanged();

            var input2 =
                    Observable.FromEventPattern(textBox2, "TextChanged").
                        Select(evt => ((TextBox)evt.Sender).Text).Throttle(TimeSpan.FromSeconds(.2)).
                        DistinctUntilChanged();

            var xs = from word in input.StartWith("")
                     from length in Task.Run(async () => { await Task.Delay(50); return word.Length; })
                     select length;

            // text for length
            var res = xs.CombineLatest(clock, (len, now) => now.ToString() + " - Word length = " + len);

            res.ObserveOn(this).Subscribe(s =>
            {
                labelLength.Text = s.ToString();
            });

            // text for combined
            var merged = input2.CombineLatest(res, (s, s2) => s2 + " " + s);

            merged.ObserveOn(this).Subscribe(s => {
                labelCombined.Text = s.ToString();
            });

            // text for filtered
            var s2Filtered = merged.Where(s => s.Contains("s"));

            s2Filtered.ObserveOn(this).Subscribe(s =>
            {
                labelFiltered.Text = s.ToString();
            });
        }
    }
}

// http://www.introtorx.com/content/v1.0.10621.0/05_Filtering.html
// http://reactivex.io/documentation/operators/filter.html
// https://msdn.microsoft.com/en-us/library/system.reactive.linq.observable(v=vs.103).aspx
// http://rxwiki.wikidot.com/101samples#toc47
// https://visualstudiomagazine.com/articles/2013/04/10/essential-reactive-extensions.aspx
// https://msdn.microsoft.com/en-us/library/hh194873(v=vs.110).aspx
// https://msdn.microsoft.com/en-us/library/hh242977(v=vs.103).aspx
// https://github.com/Reactive-Extensions/Rx.NET
// http://www.oliver-lohmann.me/an-rx-enabled-wpf-autocomplete-textbox-part-2-2/
