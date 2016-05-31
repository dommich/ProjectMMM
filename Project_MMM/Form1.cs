using System;
using System.Windows.Forms;

namespace Project_MMM
{
    public partial class Form1 : Form
    {
        int R = 5000;                           //           Ohm
        int Rd = 500;
        double U = 0.025;                       // * 10^(-3)  V
        double I = 0.00000000001;               // * 10^(-12) A
        double L = 0.0001;                      // * 10^(-6)  H
        double C = 0.001;                       // * 10^(-6)  F
        double A = 0.5;                         //            V
        int T = 1;
        int T0 = 500;                          // okres w ms
        double pi = System.Math.PI;
        int start = 0;                          // czas rozpoczęcia *10^(-6) s
        int finish = 1000;                      // czas zakończenia *10^(-6) s
        int step = 1;                           // krok obliczeń * 10^(-6) s
        double[] Source = new double[10000];
        double[] X1 = new double[10000];
        double[] X2 = new double[10000];
        double[] X3 = new double[10000];
        int wykres = 0;
        public Form1()
        {
            InitializeComponent();
            source();
            output();
            chart1_Refresh();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            R = trackBar1.Value;
            label1.Text = "R = " + R.ToString() + " Ohm";
            output();
            chart1_Refresh();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            U = trackBar2.Value;
            label2.Text = "U = " + U.ToString() + " mV";
            U *= 0.001;
            output();
            chart1_Refresh();
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            I = trackBar3.Value;
            label3.Text = "I = " + I.ToString() + " pA";
            I *= 0.00000000001;
            output();
            chart1_Refresh();
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            L = trackBar4.Value;
            label4.Text = "L = " + L.ToString() + " uH";
            L *= 0.000001;
            output();
            chart1_Refresh();
        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            C = trackBar5.Value;
            label5.Text = "C = " + C.ToString() + " uF";
            C *= 0.000001;
            output();
            chart1_Refresh();
        }
        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            A = trackBar6.Value / 2.0;
            label6.Text = "Amplitude = " + A.ToString() + " V";
            source();
            output();
            chart1_Refresh();
        }
        private void trackBar7_ValueChanged(object sender, EventArgs e)
        {
            T = trackBar7.Value;
            source();
            output();
            chart1_Refresh();
        }
        private void trackBar8_ValueChanged(object sender, EventArgs e)
        {
            start = trackBar8.Value;
            label11.Text = "Start = " + start.ToString() + " us";
            if (start < 10000) trackBar9.Minimum = start + 1;
            //source();
            output();
            chart1_Refresh();
        }
        private void trackBar9_ValueChanged(object sender, EventArgs e)
        {
            finish = trackBar9.Value;
            if(finish > 0) trackBar8.Maximum = finish - 1;
            label12.Text = "Finish = " + finish.ToString() + " us";
            //source();
            output();
            chart1_Refresh();
        }
        private void trackBar10_ValueChanged(object sender, EventArgs e)
        {
            T0 = trackBar10.Value;
            label13.Text = "Period = " + T0.ToString() + " us";
            source();
            output();
            chart1_Refresh();
        }
        private void trackBar11_ValueChanged(object sender, EventArgs e)
        {
            step = trackBar11.Value;
            label14.Text = "Step = " + step.ToString() + " us";
            output();
            chart1_Refresh();
        }
        private void trackBar12_ValueChanged(object sender, EventArgs e)
        {
            Rd = trackBar12.Value;
            label15.Text = "Rd = " + Rd.ToString() + " Ohm";
            output();
            chart1_Refresh();
        }
        // Input voltage.
        private void source()
        {
            Source[0] = 0;
            for (int i = 0; i < 10000; i++)
            {
                switch (T)
                {
                    case 0:
                        if(i!=0) Source[i] = A * Math.Sin(2 * pi / T0 * i);
                        break;
                    case 1:
                        if(i!=0) Source[i] = (i % (T0) >= 0.5*T0) ? -A : A;
                        break;
                    case 2:
                        if (i < 9999) Source[i + 1]= ((i+T0/4) % (T0) >= 0.5 * T0) ? (Source[i] - 4*A/T0) : (Source[i] + 4*A/T0);
                        break;
                }
            }
        }
        //funkcja zwracająca wartosc funkcji pradu i1 płynącego przez diodę w punkcie x
        double funkcja(double x, double Uc, double Uwe)
        {
            double wynik = x - I * (System.Math.Exp((Uwe - x * R - Uc) / U) - 1);
	        return wynik;
        }
        //funkcja zwracjąca wartosć pochodnej funkcji prądu i1 w punkcie x
        double pochodna(double x, double Uc, double Uwe)
        {
            double wynik = 1 - I * System.Math.Exp((Uwe - x * R - Uc) / U) * (-R * U) / (U * U);
	        return wynik;
        }
        //rozwiązywanie nieliniowego równania prądu i1 metodą Newtona-Raphsona
        double i1(double Uc, double Uwe, double i1)
        {
	        double x0,x1,f1,f0;
	        int i;

	        if((Uwe-i1*R-Uc)<=0) return 0;
	        else
	        {
		        //punkt startowy!!!
		        x0=10;
	
		        //x1 = x0 - 1; 
		        f0 = funkcja(x0,Uc,Uwe); 
		        i = 100;
  		        do
		        {
    		        f1 = pochodna(x0,Uc,Uwe);
    		        x1 = x0;
    		        x0 = x0 - f0 / f1;
    		        f0 = funkcja(x0,Uc,Uwe);
    		        i--;
  		        } while(i>0);

            return x0;
  	        }
        }
        // Output voltage.
        private void output()
        {
            X1[0] = X2[0] = X3[0] = 0;
            for (int i = 1; i < finish; i++)
            {
                X1[i] = i1(X2[i - 1], Source[i - 1], X1[i - 1]);
                X2[i] = X2[i - 1] + 0.000001 * step * ((1 / C) * X1[i] - X2[i - 1] / (C * Rd) - (1 / C) * X3[i - 1]);
                X3[i] = X3[i - 1] + 0.000001 * step * (1 / L * X2[i - 1]);
            }
            for (int i = 1; i < finish; i++)
            {
                X1[i] *= 1000000;
                X2[i] *= 1000000 / step;
                X3[i] *= 1000000 / step;
            }
        }
        // Draw chart.
        private void chart1_Refresh()
        {
            // Add series.   
            string[] seriesArray = { "Series1" };
            chart1.Series.Clear();
            System.Windows.Forms.DataVisualization.Charting.Series series = this.chart1.Series.Add(seriesArray[0]);
            series.ChartArea = "ChartArea1";    
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            
            // Add points.
                
            chart1.Series["Series1"].Points.Clear();
            switch (wykres)
            {
                case 0:
                    series.Points.Add(Source[start]);
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Source voltage");
                    label16.Text = "[V]";
                    break;
                case 1:
                    series.Points.Add(X1[start]);
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Diode current");
                    label16.Text = "[pA]";
                    break;
                case 2:
                    series.Points.Add(X2[start]);
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Condensator voltage");
                    label16.Text = "[pV]";
                    break;
                case 3:
                    series.Points.Add(X3[start]);
                    chart1.Titles.Clear();
                    chart1.Titles.Add("Coil current");
                    label16.Text = "[pA]";
                    break;
            }
            for (int i = start+1; i < finish; i++)
            {
                switch (wykres)
                {
                    case 0: series.Points.Add(Source[i]);   break;
                    case 1: series.Points.Add(X1[i]);       break;
                    case 2: series.Points.Add(X2[i]);       break;
                    case 3: series.Points.Add(X3[i]);       break;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (wykres == 0) wykres = 3;
            else wykres--;
            chart1_Refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            wykres = (wykres + 1) % 4;
            chart1_Refresh();
        }
    }
}