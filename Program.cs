using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.SolverFoundation;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.SolverFoundation.Services;
using System.Text.RegularExpressions;

namespace MatProJe
{
    class Program
    {
        static void Main(string[] args)
        {
            
            DosyaIslemi b = new DosyaIslemi();
            b.DosyaOkuma();
            Simplex a=new Simplex();
            
            
            for (int i=0;i<b.t.Count;i++)//her t yılı için tekrar simplex çözücek (çünkü gelen değerler sürekli değişecek)
            {               
                a.SimplexCozucu(b.t[i],b.D[i],b.E[i],b.c[i],b.n[i]);//metoda dosya okuma dan gelen değerleri gönderiyoruz
            }
            Console.WriteLine("Sonuc = " + a.Sonuc);
            Console.ReadKey();
            
        }
       
    }
    class Simplex
    {
        private double w;
        private double z;
        private double sonuc;

        public double Sonuc
        {
            get
            {
                return sonuc;
            }
            set
            {
                sonuc = value;
            }
        }
        public double W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;
            }
        }
        

        public double Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public Simplex()//constructor
        {
            
        }

        public void SimplexCozucu(double t, double D, double E, double c, double n)//girilen değerlere göre kütüphaneyi kullanarak simplex i çözücek
        {
            
           if(t==1)
            {
                W = 0;
                Z = 0;
                Sonuc = 0;
            }          
            
            var solver = SolverContext.GetContext();
            var model = solver.CreateModel();
            

            var decisionX = new Decision(Domain.IntegerNonnegative, "X");
            var decisionY = new Decision(Domain.IntegerNonnegative, "Y");

            //simplex e göre belirlencek x değişkeni ve y değişkeni
            model.AddDecision(decisionX);
            model.AddDecision(decisionY);

            //minimalize edilecek hedef fonksiyonunu yazıyoruz
            model.AddGoal("Hedef", GoalKind.Minimize,
                (c * decisionX) + (n * decisionY));

            
            //kısıtları giriyoruz
            model.AddConstraint("Kisit0", decisionX+W + decisionY+Z+ E >= D);
            model.AddConstraint("Kisit1", decisionY+Z <= 0.2 * (decisionX+W+ decisionY+Z + E));

            //solver a bu kısıtlara göre solver ın Solve(Çöz) adlı metodunu çağırıyoruz
            var solution = solver.Solve();

            //Gelen değerlerin double a dönüştürülmüş halini iki adet double değişkene eşitliyoruz
            double x = decisionX.GetDouble();
            double y = decisionY.GetDouble();

            Console.WriteLine("X"+t+" =" + x + " Y" +t + " =" + y);
           

            //Sonuc her iterasyonda fonksiyonun o yıldaki minimum değerini veriyor
            Sonuc += (c * x) + (n * y);
            
            if (t%20!=0)
            {
                W += x;               
            }
            else
            {
                W = 0;
            }
            if(t%15!=0)
            {
                Z += y;              
            }
            else
            {
                Z = 0;
            }
            
            
            solver.ClearModel();
            
        }
    }
    class DosyaIslemi
    {

        //Dosyadan alınacak değerler bu listelerde tutulacak
        public List<double> t = new List<double>();
        public List<double> D = new List<double>();
        public List<double> E = new List<double>();
        public List<double> c = new List<double>();
        public List<double> n = new List<double>();
        //listler duruma göre değişebilir , biz dikey alalım dedik D,E,c,n,,,,, yatay almamız gerekedebilir
        public void DosyaOkuma()//dosyayı okuyup list leri doldurcaz
        {

            string[] okunan;
            string satır;
            FileStream akis;
            StreamReader Okuma;
            string Yol = "simplexGirdisi.txt";//yol parametreden gelen kullanıcının attığı değere eşitleniyo
            akis = new FileStream(Yol, FileMode.Open, FileAccess.Read);
            Okuma = new StreamReader(akis);
            satır = Okuma.ReadLine(); 
            satır = Okuma.ReadLine();

            while (satır != null)
            {
                okunan = Regex.Split(satır, @"\s+");
                t.Add(double.Parse(okunan[0]));
                D.Add(double.Parse(okunan[1]));
                E.Add(double.Parse(okunan[2]));
                c.Add(double.Parse(okunan[3]));
                n.Add(double.Parse(okunan[4]));

                satır = Okuma.ReadLine();

            }
            akis.Close();
        }
    }
}
