using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Sockets;
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

                //WypiszOsobniki(osobniki);

                //Tworzenie parametrów


                TworzParametry(osobniki,liczbaParametrow,-1,2,liczbaLbnp);

                //var counter = 0;
                //for (int i = 0; i < osobniki.Count; i++)
                //{
                //    for (int j = 0; j < liczbaParametrow; j++)
                //    {
                //        var cb = new List<int>();
                //        for (int k = 0; k < liczbaLbnp; k++)
                //        {
                //            cb.Add(osobniki[i].Chromosomy[counter]);
                //            counter++;
                //        }
                //        osobniki[i].Parametry.Add(DekodujParametry(cb, -1, 2, liczbaLbnp));
                //    }
                //    counter = 0;
                //}

                //Wypisanie Osobników z parametrami

                WypiszOsobnikiZParametrami(osobniki);

                //Liczenie funkcji przystosowania

                for (int i = 0; i < osobniki.Count; i++)
                {
                    osobniki[i].WartoscFunkcjiPrzystosowania =
                        PoliczFunkcjePrzystosowania(osobniki[i].Parametry[0], osobniki[i].Parametry[1]);
                }

                //Wypisanie wszystkich wartosci funkcji przystosowania

                WypiszFunkcjePrzystosowania(osobniki);

                //Wypisanie funkcji przystosowania najlepszej i sredniej wartosci tych funkcji
                //algorytm genetyczny
                WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMax(osobniki);
                WypiszSredniaFunkcjiPrzystosowania(osobniki);

                Console.WriteLine("-----------");
                var nowaPopulacja = new List<Osobnik>();
                for (int i = 0; i < liczbaIteracji; i++)
                {
                   nowaPopulacja = OperatorSelekcjiTurniejowej(osobniki, rozmiarTurnieju, "Max");

                   for (int j = 0; j < nowaPopulacja.Count; j++)
                   {
                       //operator mutacji
                       nowaPopulacja[j].Chromosomy = OperatorMutacjiJednopunktowej(nowaPopulacja[j].Chromosomy, nowaPopulacja[j].Chromosomy.Count);
                   }

                    // Operator Hot Deck

                    var najlepszyZeStarejPuli = OperatorHotDeck(osobniki);

                    nowaPopulacja.Add(najlepszyZeStarejPuli);

                    //Dekodowanie i nowa funkcja przystosowania

                    TworzParametry(nowaPopulacja, liczbaParametrow, 0, 100, liczbaLbnp);
                    for (int j = 0; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].WartoscFunkcjiPrzystosowania =
                            PoliczFunkcjePrzystosowania(nowaPopulacja[j].Parametry[0], nowaPopulacja[j].Parametry[1]);
                    }

                    //Wypisanie najlepszej i sredniej wartosci funkcji

                    WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMax(nowaPopulacja);
                    Console.WriteLine();
                    WypiszSredniaFunkcjiPrzystosowania(nowaPopulacja);
                    Console.WriteLine();


                    // zastap stara pule osobnikow nowa pula

                    var counter = 0;
                    foreach (Osobnik osobnik in nowaPopulacja)
                    {
                        osobniki[counter] = osobnik;
                        counter++;
                    }
                    
                    //Wyczyść zmienną nowaPopulacja

                    nowaPopulacja.Clear(); // czyści listę (Count)
                    nowaPopulacja.TrimExcess(); //czyści listę (Capacity)
                    //Console.WriteLine($"wykonano juz razy {i+1}");
                    //Console.WriteLine();
                }

                var najlepszyOsobnik = OperatorHotDeck(osobniki);

                Console.WriteLine(najlepszyOsobnik);

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
        }

        public static void WypiszOsobnikiZParametrami(List<Osobnik> osobniki)
        {
            for (int i = 0; i < osobniki.Count; i++)
            {
                Console.WriteLine($"Osobnik {i + 1}");
                Console.WriteLine("");
                for (int j = 0; j < osobniki[i].Parametry.Count; j++)
                {
                    Console.WriteLine($"Parametr {j + 1}");
                    Console.WriteLine(osobniki[i].Parametry[j]);
                }
                Console.WriteLine("----------------------");
            }
        }

        public static void WypiszFunkcjePrzystosowania(List<Osobnik> osobniki)
        {
            for (int i = 0; i < osobniki.Count; i++)
            {
                Console.WriteLine($"Osobnik {i + 1}");
                Console.WriteLine($"wartosc funkcji przystosowania: {osobniki[i].WartoscFunkcjiPrzystosowania}");
                Console.WriteLine("");
            }
        }

        public static void WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMax(List<Osobnik> osobniki)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < osobniki.Count; i++)
            {
                result.Add(osobniki[i].WartoscFunkcjiPrzystosowania);
            }

            Console.WriteLine("Najlepsza wartosc funkcji przystosowania:");
            Console.WriteLine(result.Max());
        }

        public static void WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMin(List<Osobnik> osobniki)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < osobniki.Count; i++)
            {
                result.Add(osobniki[i].WartoscFunkcjiPrzystosowania);
            }

            Console.WriteLine("Najlepsza wartosc funkcji przystosowania:");
            Console.WriteLine(result.Min());
        }

        public static void WypiszSredniaFunkcjiPrzystosowania(List<Osobnik> osobniki)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < osobniki.Count; i++)
            {
                result.Add(osobniki[i].WartoscFunkcjiPrzystosowania);
            }

            Console.WriteLine("Srednia wartosc funkcji przystosowania:");
            Console.WriteLine(result.Average());
        }

        public static void TworzParametry(List<Osobnik> osobniki, int liczbaParametrow, int zdMin, int zdMax, int liczbaLbnp)
        {
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

                    if (osobniki[i].Parametry.Count == liczbaParametrow)
                    {
                        osobniki[i].Parametry[j] = DekodujParametry(cb, zdMin, zdMax, liczbaLbnp);
                    }
                    else
                    {
                        osobniki[i].Parametry.Add(DekodujParametry(cb, zdMin, zdMax, liczbaLbnp));
                    }
                }
                counter = 0;
            }
        }

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

        public static double PoliczFunkcjePrzystosowania(double x1, double x2)
        {
            var result = Math.Sin(x1 * 0.05) + Math.Sin(x2 * 0.05) + 0.4 * Math.Sin(x1 * 0.15) * Math.Sin(x2 * 0.15);

            return result;
        }

        public static List<Osobnik> OperatorSelekcjiTurniejowej(List<Osobnik> osobniki, int RozmiarTurnieju, string maxCzyMin)
        {
            var random = new Random();
            var wybrany = osobniki[0];
            var nowaPula = new List<Osobnik>();

            for (int i = 0; i < osobniki.Count-1; i++)
            {
                var turniej = new List<Osobnik>();

                var counter = 0;
                while (counter < RozmiarTurnieju)
                {
                    int losowyIndex = random.Next(osobniki.Count);
                    if (turniej.Select(x=>x.Id).Contains(osobniki[losowyIndex].Id))
                    {
                        continue;
                    }
                    turniej.Add(osobniki[losowyIndex]);
                    counter++;
                }

                double max = 0;
                double min = 9999999999;
                if (maxCzyMin == "Max")
                {
                    foreach (var osobnik in turniej)
                    {
                        if (osobnik.WartoscFunkcjiPrzystosowania > max)
                        {
                            max = osobnik.WartoscFunkcjiPrzystosowania;
                            wybrany = new Osobnik(osobnik);
                        }
                    }
                }
                else
                {
                    foreach (var osobnik in turniej)
                    {
                        if (osobnik.WartoscFunkcjiPrzystosowania < min)
                        {
                            min = osobnik.WartoscFunkcjiPrzystosowania;
                            wybrany = new Osobnik(osobnik);
                        }
                    }
                }
                nowaPula.Add(wybrany);
            }

            return nowaPula;
        }

        public static List<int> OperatorMutacjiJednopunktowej(List<int> cb, int lBnCh)
        {
            var result = cb;
            var random = new Random();
            var b_punkt = random.Next(cb.Count);

            if (result[b_punkt] == 0)
            {
                result[b_punkt] = 1;
            }
            else
            {
                result[b_punkt] = 0;
            }

            return result;
        }

        public static Osobnik OperatorHotDeck(List<Osobnik> osobniki)
        {
            var indexNajlepszego = 0;
            for (int j = 0; j < osobniki.Count; j++)
            {
                if (osobniki[j].WartoscFunkcjiPrzystosowania > osobniki[indexNajlepszego].WartoscFunkcjiPrzystosowania)
                {
                    indexNajlepszego = j;
                }
            }

            return osobniki[indexNajlepszego];
        }

        public class Osobnik
        {
            private readonly Random _random = new Random();

            public Guid Id;
            public List<int> Chromosomy = new List<int>();
            public List<double> Parametry = new List<double>();
            public int liczbaChromosomow;
            public int liczbaParametrow;
            public int Lbnp; //liczba bitów na parametr
            public double WartoscFunkcjiPrzystosowania;

            public Osobnik(int Lbnp, int liczbaParametrow)
            {
                this.Id = Guid.NewGuid();
                this.Lbnp = Lbnp;
                this.liczbaParametrow = liczbaParametrow;
                this.liczbaChromosomow = liczbaParametrow * Lbnp;
                for (int i = 0; i < liczbaChromosomow; i++)
                {
                    this.Chromosomy.Add(_random.Next(2));
                }
            }

            public Osobnik(Osobnik nowyOsobnik)
            {
                this.Id = Guid.NewGuid();
                this.Chromosomy = new List<int>(nowyOsobnik.Chromosomy);
                this.Parametry = new List<double>(nowyOsobnik.Parametry);
                this.liczbaChromosomow = nowyOsobnik.liczbaChromosomow;
                this.liczbaParametrow = nowyOsobnik.liczbaParametrow;
                this.Lbnp = nowyOsobnik.Lbnp;
                this.WartoscFunkcjiPrzystosowania = nowyOsobnik.WartoscFunkcjiPrzystosowania;
            }

            public override string ToString()
            {
                return $"Najlepszy Osobnik: {this.Id}\n Parametry: {string.Join(" ",this.Parametry)}\n Wartosc Funkcji Przystosowania: {this.WartoscFunkcjiPrzystosowania}"; //Pomaga wyświetlić koncowy wynik
            }
        }
       
    }
}
