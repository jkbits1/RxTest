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

            //var input3 =
            //        Observable.FromEventPattern(textBox3, "TextChanged").
            //            Select(evt => ((TextBox)evt.Sender).Text).Throttle(TimeSpan.FromSeconds(.2)).
            //            DistinctUntilChanged();

            var xs = from word in input.StartWith("")
                     from length in Task.Run(async () => { await Task.Delay(50); return word.Length; })
                     select length;

            var res = xs.CombineLatest(clock, (len, now) => now.ToString() + " - Word length = " + len);

            res.ObserveOn(this).Subscribe(s =>
            {
                //var s2 = input2;

                label1.Text = s.ToString();
            });

            //var merged = input2.Merge(res);
            //var merged = input2.CombineLatest(res, (s, s2) => "combined-" + s2 + " " + s);
            var merged = input2.CombineLatest(res, (s, s2) => s2 + " " + s);

            //input2.ObserveOn(this).Subscribe(s2 => {
            merged.ObserveOn(this).Subscribe(s => {
                label2.Text = s.ToString();
            });

            var s2Filtered = merged.Where(s => s.Contains("s"));

            s2Filtered.ObserveOn(this).Subscribe(s =>
            {
                label3.Text = 
                    //"filtered on x -" + 
                    s.ToString();
            });
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
