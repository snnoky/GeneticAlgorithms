using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;

namespace Zad4MIW
{
    class Program
    {
        static void Main(string[] args)
        {
            

            Console.WriteLine("Wybierz zadanie: 1/2/3");
            var currentProgram = Console.ReadLine();

            if (currentProgram == "1")
            {
                var liczbaParametrow = 2;
                var liczbaLbnp = 3;
                var liczbaIteracji = 20;
                var liczbaOsobnikow = 9;
                var rozmiarTurnieju = 2;
                var osobniki = new List<Osobnik>();

                //Console.WriteLine("Podaj liczbe parametrow:");
                //liczbaParametrow = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Podaj liczbe bitow na parametr: minimum 3");
                liczbaLbnp = Convert.ToInt32(Console.ReadLine());
                if (liczbaLbnp < 3)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=3");
                    liczbaLbnp = 3;
                }

                Console.WriteLine("Podaj liczbe iteracji");
                liczbaIteracji = Convert.ToInt32(Console.ReadLine());
                if (liczbaIteracji < 20)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=20");
                    liczbaIteracji = 20;
                }

                Console.WriteLine("Podaj liczbe osobnikow");
                liczbaOsobnikow = Convert.ToInt32(Console.ReadLine());
                if (liczbaOsobnikow%2 == 0)
                {
                    Console.WriteLine("liczba osobników ma być nieparzysta, użyte zostanie min=9");
                    liczbaOsobnikow = 9;
                }
                if (liczbaOsobnikow < 9)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=9");
                    liczbaOsobnikow = 9;
                }

                Console.WriteLine("Podaj rozmiar turnieju");
                rozmiarTurnieju = Convert.ToInt32(Console.ReadLine());
                if (rozmiarTurnieju < 2 || rozmiarTurnieju > liczbaOsobnikow*0.2)
                {
                    Console.WriteLine("Niepoprawny rozmiar użyty zostanie rozmiar=2");
                    rozmiarTurnieju = 2;
                }
                
                for (int i = 0; i < liczbaOsobnikow; i++)
                {
                    Osobnik os = new Osobnik(liczbaLbnp,liczbaParametrow);
                    osobniki.Add(os);
                }

                WypiszOsobniki(osobniki);

                //for (int i = 0; i < osobniki.Count; i++)
                //{
                //    var cb = new List<int>();
                //    for (int j = 0; j < osobniki[i].Chromosomy.Count; j++)
                //    {
                //        for (int k = 0; k < liczbaLbnp; k++)
                //        {
                //            cb.Add(osobniki[i].Chromosomy[k]);
                //        }

                //        for (int k = 0; k < liczbaParametrow; k++)
                //        {
                //            osobniki[i].Parametry.Add(DekodujParametry(cb,-1,2,liczbaLbnp));
                //        }
                //    }
                //}


                //Tworzenie parametrów

                var counter = 0;
                for (int i = 0; i < osobniki.Count; i++)
                {
                    for (int j = 0; j < liczbaParametrow; j++)
                    {
                        var cb = new List<int>();
                        for (int k = 0; k < liczbaLbnp; k++)
                        {
                            cb.Add(osobniki[i].Chromosomy[counter]);
                            counter++;
                        }
                        osobniki[i].Parametry.Add(DekodujParametry(cb, -1, 2, liczbaLbnp));
                    }
                    counter = 0;
                }

                //Wypisanie Osobników z parametrami

                for (int i = 0; i < osobniki.Count; i++)
                {
                    Console.WriteLine($"Osobnik {i+1}");
                    Console.WriteLine("");
                    for (int j = 0; j < osobniki[i].Parametry.Count; j++)
                    {
                        Console.WriteLine($"Parametr {j+1}");
                        Console.WriteLine(osobniki[i].Parametry[j]);
                    }
                    Console.WriteLine("----------------------");
                }

            }
            else if (currentProgram == "2")
            {

            }
            else if (currentProgram == "3")
            {

            }
            else
            {
                Console.WriteLine("Nic nie wybrano.");
            }


            /*
             klasa osobnik sklada sie z paru rzeczy (na poczatek tworzy sie z parametrow, potem dekoduje zeby zamienic z binarki na double)
             klasa parametr z lista int 0 lub 1 o dlugosci z zadania np w config to dac
             klasa dekodowane parametry z wartoscia double
             zmienna liczba bitow na parametr mozna dac w petle(ustawic z gory wart minimalna)
             reszta funkcji
             operator selekcji turniejowej losuje tak zeby dac roznym szanse
             organizowac zawody kilka razy
             wybierac z tych co sa w zawodach
             wynikiem jest najsilniejszy czyli ten z najwieksza wartoscia funkcji przystosowania
             robic to tyle ile jest liczba iteracji albo po bledzie czyli jak licze funkcje przystosowania i wychodzi 
             taka liczba jaka podana w zadaniu (osobnik rowny wartosci przystosowania z zadania)
             zeby sprawdzic wagi podac wartosc xora jezeli wychodzi powyzej 0,9 to dobrze i jak ponizej 0,1 to tez spoko
             zrobic tak zeby praktycznie dawalo 0 albo 1
            */
        }

        public static void WypiszOsobniki(List<Osobnik> osobniki)
        {
            for (int i = 0; i < osobniki.Count; i++)
            {
                Console.WriteLine($"Osobnik {i + 1}");
                for (int j = 0; j < osobniki[i].Chromosomy.Count; j++)
                {
                    Console.WriteLine(osobniki[i].Chromosomy[j]);
                }

                Console.WriteLine("---------------");
            }

            //Console.WriteLine("-------------");

            //for (int i = 0; i < osobniki.Count; i++)
            //{
            //    for (int j = 0; j < osobniki[i].Parametry.Count; j++)
            //    {
            //        Console.WriteLine($"Parametr {j}");
            //        for (int k = 0; k < osobniki[i].Parametry[j].chromosomy.Count; k++)
            //        {
            //            Console.WriteLine(osobniki[i].Parametry[j].chromosomy[k]);

            //        }
            //    }
            //    Console.WriteLine("-------------");

            //}
        }

        //public class Parametr
        //{
        //    public List<int> chromosomy = new List<int>();
        //}
        public static double DekodujParametry(List<int> cb, int zdMin, int zdMax, int lbnp)
        {
            int zd = zdMax - zdMin;

            double parametr;
            double ctmp = 0;

            for (int i = 0; i < cb.Count; i++)
            {
                ctmp += cb[i] * Math.Pow(2, i);
            }

            parametr = zdMin + (ctmp / (Math.Pow(2, lbnp) - 1)) * zd;

            return parametr;
        }
        public class Osobnik
        {
            private readonly Random _random = new Random();

            public List<int> Chromosomy = new List<int>();
            //public List<Parametr> Parametry =  new List<Parametr>();
            public List<double> Parametry = new List<double>();
            public int liczbaChromosomow;
            public int liczbaParametrow;
            public int Lbnp; //liczba bitów na parametr
            public double WartoscFunkcjiPrzystosowania;

            public Osobnik(int Lbnp, int liczbaParametrow)
            {
                this.Lbnp = Lbnp;
                this.liczbaParametrow = liczbaParametrow;
                this.liczbaChromosomow = liczbaParametrow * Lbnp;
                for (int i = 0; i < liczbaChromosomow; i++)
                {
                    this.Chromosomy.Add(_random.Next(2));
                }

                //for (int i = 0; i < liczbaParametrow; i++)
                //{
                //    var parametr = new Parametr();
                //    for (int j = 0; j < liczbaChromosomow; j++)
                //    {
                //        parametr.chromosomy.Add(Chromosomy[j]);
                //        if (parametr.chromosomy.Count == Lbnp)
                //        {
                //            Parametry.Add(parametr);
                //            break;
                //        }
                //    }
                //    Parametry.Add(parametr);
                //}

            }

        }
       
    }
}
