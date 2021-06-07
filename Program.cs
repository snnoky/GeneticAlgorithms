using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
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
                
                //tworzenie osobnikow

                for (int i = 0; i < liczbaOsobnikow; i++)
                {
                    Osobnik os = new Osobnik(liczbaLbnp,liczbaParametrow);
                    osobniki.Add(os);
                }

                //WypiszOsobniki(osobniki);

                //Tworzenie parametrów


                TworzParametry(osobniki,liczbaParametrow,0,100,liczbaLbnp);

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
                        PoliczFunkcjePrzystosowaniaZad1(osobniki[i].Parametry[0], osobniki[i].Parametry[1]);
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

                    var najlepszyZeStarejPuli = OperatorHotDeck(osobniki, "Max");

                    nowaPopulacja.Add(najlepszyZeStarejPuli);

                    //Dekodowanie i nowa funkcja przystosowania

                    TworzParametry(nowaPopulacja, liczbaParametrow, 0, 100, liczbaLbnp);
                    for (int j = 0; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].WartoscFunkcjiPrzystosowania =
                            PoliczFunkcjePrzystosowaniaZad1(nowaPopulacja[j].Parametry[0], nowaPopulacja[j].Parametry[1]);
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

                var najlepszyOsobnik = OperatorHotDeck(osobniki, "Max");

                Console.WriteLine(najlepszyOsobnik);

            }
            else if (currentProgram == "2")
            {
                var liczbaParametrow = 3;
                var liczbaLbnp = 4;
                var liczbaIteracji = 100;
                var liczbaOsobnikow = 13;
                var rozmiarTurnieju = 3;
                var osobniki = new List<Osobnik>();



                //otwieranie pliku sinusik.txt
                //pobieranie wartosci z sinusik.txt
                //wyliczenie linii w sinusik.txt
                var filePath = "../../../sinusik.txt";
                var fileLinesCount = 0;
                string[] fileItems = new string[] { };
                List<double> xItems = new List<double>();
                List<double> yItems = new List<double>();

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Nie ma takiego pliku!");
                }
                else
                {
                    fileLinesCount = File.ReadAllLines(filePath).Length;
                    fileItems = File.ReadAllLines(filePath);
                }


                foreach (string fileItem in fileItems)
                {
                    
                    xItems.Add(Convert.ToDouble(fileItem.Split(" ")[0]));
                    yItems.Add(Convert.ToDouble(fileItem.Split(" ")[1]));
                }

                //Console.WriteLine("Podaj liczbe parametrow:");
                //liczbaParametrow = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Podaj liczbe bitow na parametr: minimum 4");
                liczbaLbnp = Convert.ToInt32(Console.ReadLine());
                if (liczbaLbnp < 4)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=4");
                    liczbaLbnp = 4;
                }

                Console.WriteLine("Podaj liczbe iteracji");
                liczbaIteracji = Convert.ToInt32(Console.ReadLine());
                if (liczbaIteracji < 100)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=100");
                    liczbaIteracji = 100;
                }

                //tworzenie osobnikow

                for (int i = 0; i < liczbaOsobnikow; i++)
                {
                    Osobnik os = new Osobnik(liczbaLbnp, liczbaParametrow);
                    osobniki.Add(os);
                }

                //Tworzenie parametrów


                TworzParametry(osobniki, liczbaParametrow, 0, 3, liczbaLbnp);

                //Liczenie funkcji przystosowania

                for (int i = 0; i < osobniki.Count; i++)
                {
                    osobniki[i].WartoscFunkcjiPrzystosowania =
                        PoliczFunkcjePrzystosowaniaZad2(osobniki[i].Parametry[0], osobniki[i].Parametry[1], osobniki[i].Parametry[2], xItems, yItems, fileLinesCount);
                }

                //Wypisanie funkcji przystosowanie najlepszego osobnika i średniej przed iteracjami
                Console.WriteLine("Przed:");
                WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMin(osobniki);
                WypiszSredniaFunkcjiPrzystosowania(osobniki);
                Console.WriteLine("-----------------");
                //Iteracje Algorytmu

                var nowaPopulacja = new List<Osobnik>();
                for (int i = 0; i < liczbaIteracji; i++)
                {
                    //Selekcja turniejowa
                    nowaPopulacja = OperatorSelekcjiTurniejowej(osobniki, rozmiarTurnieju, "Min");

                    //operator krzyzowania na nowej puli
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp,0,1); // Pierwsze dwa osobniki
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, 2, 3); // trzeci i czwarty osobnik
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, 8, 9); // dziewiąty i dziesiąty
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, nowaPopulacja.Count-2, nowaPopulacja.Count-1); // ostatnie dwa

                    //operator mutacji jednopunktowej od 5 do ostatniego osobnika
                    for (int j = 4; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].Chromosomy = OperatorMutacjiJednopunktowej(nowaPopulacja[j].Chromosomy, nowaPopulacja[j].Chromosomy.Count);
                    }

                    // Operator Hot Deck

                    var najlepszyZeStarejPuli = OperatorHotDeck(osobniki, "Min");

                    nowaPopulacja.Add(najlepszyZeStarejPuli);

                    //Dekodowanie i nowa funkcja przystosowania

                    TworzParametry(nowaPopulacja, liczbaParametrow, 0, 3, liczbaLbnp);
                    for (int j = 0; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].WartoscFunkcjiPrzystosowania =
                            PoliczFunkcjePrzystosowaniaZad2(nowaPopulacja[j].Parametry[0], nowaPopulacja[j].Parametry[1], nowaPopulacja[j].Parametry[2], xItems, yItems, fileLinesCount);
                    }

                    //Wypisanie funkcji przystosowanie najlepszego osobnika i średniej
                    Console.WriteLine();
                    Console.WriteLine($"Iteracja {i+1}");
                    WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMin(nowaPopulacja);
                    WypiszSredniaFunkcjiPrzystosowania(nowaPopulacja);

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
                    
                }

                
                var najlepszyOsobnik = OperatorHotDeck(osobniki, "Min");

                Console.WriteLine(najlepszyOsobnik);

            }
            else if (currentProgram == "3")
            {
                var liczbaParametrow = 9; //3*3(3neurony, 3 wagi na neuron)
                var liczbaLbnp = 4; // takie jak w poprzednim zadaniu
                var liczbaIteracji = 100;// takie jak w poprzednim zadaniu
                var liczbaOsobnikow = 13;// takie jak w poprzednim zadaniu
                var rozmiarTurnieju = 3;// takie jak w poprzednim zadaniu
                var osobniki = new List<Osobnik>();


                Console.WriteLine("Podaj liczbe bitow na parametr: minimum 4");
                liczbaLbnp = Convert.ToInt32(Console.ReadLine());
                if (liczbaLbnp < 4)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=4");
                    liczbaLbnp = 4;
                }

                Console.WriteLine("Podaj liczbe iteracji");
                liczbaIteracji = Convert.ToInt32(Console.ReadLine());
                if (liczbaIteracji < 100)
                {
                    Console.WriteLine("Poniżej min użyte zostanie min=100");
                    liczbaIteracji = 100;
                }

                //tworzenie osobnikow

                for (int i = 0; i < liczbaOsobnikow; i++)
                {
                    Osobnik os = new Osobnik(liczbaLbnp, liczbaParametrow);
                    osobniki.Add(os);
                }

                //Tworzenie parametrów


                TworzParametry(osobniki, liczbaParametrow, -10, 10, liczbaLbnp);


                //Liczenie funkcji przystosowania

                for (int i = 0; i < osobniki.Count; i++)
                {
                    osobniki[i].WartoscFunkcjiPrzystosowania =
                        PoliczFunkcjePrzystosowaniaZad3(osobniki, i);
                }

                //Wypisanie funkcji przystosowanie najlepszego osobnika i średniej przed iteracjami
                Console.WriteLine("Przed:");
                WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMin(osobniki);
                WypiszSredniaFunkcjiPrzystosowania(osobniki);
                Console.WriteLine("-----------------");
                //Iteracje Algorytmu

                var nowaPopulacja = new List<Osobnik>();
                for (int i = 0; i < liczbaIteracji; i++)
                {
                    //Selekcja turniejowa
                    nowaPopulacja = OperatorSelekcjiTurniejowej(osobniki, rozmiarTurnieju, "Min");

                    //operator krzyzowania na nowej puli
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, 0, 1); // Pierwsze dwa osobniki
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, 2, 3); // trzeci i czwarty osobnik
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, 8, 9); // dziewiąty i dziesiąty
                    OperatorKrzyzowania(nowaPopulacja, liczbaLbnp, nowaPopulacja.Count - 2, nowaPopulacja.Count - 1); // ostatnie dwa

                    //operator mutacji jednopunktowej od 5 do ostatniego osobnika
                    for (int j = 4; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].Chromosomy = OperatorMutacjiJednopunktowej(nowaPopulacja[j].Chromosomy, nowaPopulacja[j].Chromosomy.Count);
                    }

                    // Operator Hot Deck

                    var najlepszyZeStarejPuli = OperatorHotDeck(osobniki, "Min");

                    nowaPopulacja.Add(najlepszyZeStarejPuli);

                    //Dekodowanie i nowa funkcja przystosowania

                    TworzParametry(nowaPopulacja, liczbaParametrow, -10, 10, liczbaLbnp);
                    for (int j = 0; j < nowaPopulacja.Count; j++)
                    {
                        nowaPopulacja[j].WartoscFunkcjiPrzystosowania =
                            PoliczFunkcjePrzystosowaniaZad3(nowaPopulacja, j);
                    }

                    //Wypisanie funkcji przystosowanie najlepszego osobnika i średniej
                    Console.WriteLine();
                    Console.WriteLine($"Iteracja {i + 1}");
                    WypiszFunkcjePrzystosowaniaNajlepszegoOsobnikaMin(nowaPopulacja);
                    WypiszSredniaFunkcjiPrzystosowania(nowaPopulacja);

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

                }

                var najlepszyOsobnik = OperatorHotDeck(osobniki, "Min");

                Console.WriteLine(najlepszyOsobnik);
            }
            else
            {
                Console.WriteLine("Nic nie wybrano.");
            }
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

        public static double PoliczFunkcjePrzystosowaniaZad1(double x1, double x2)
        {
            var result = Math.Sin(x1 * 0.05) + Math.Sin(x2 * 0.05) + 0.4 * Math.Sin(x1 * 0.15) * Math.Sin(x2 * 0.15);

            return result;
        }

        public static double PoliczFunkcjePrzystosowaniaZad2(double pa, double pb, double pc, List<double> x, List<double> y, int lineCount )
        {

            double result = 0;

            for (int i = 0; i < lineCount; i++)
            {
                var error = pa * Math.Sin(pb * x[i] + pc);
                result += Math.Pow(y[i] - error, 2);
            }

            return result;
        }

        public static double PoliczFunkcjePrzystosowaniaZad3(List<Osobnik> osobniki, int ktoryOsobnik)
        {
            double result = 0;

            double xor0 = 0;
            double xor1 = 1;

            double d1 = 0;
            double d2 = 1;
            double d3 = 1;
            double d4 = 0;

            double s1 = 0;
            double s2 = 0;
            double s3 = 0;

            double s4 = 0;
            double s5 = 0;
            double s6 = 0;

            double s7 = 0;
            double s8 = 0;
            double s9 = 0;

            double s10 = 0;
            double s11 = 0;
            double s12 = 0;

            double wynik1 = 0;
            double wynik2 = 0;
            double wynik3 = 0;
            double wynik4 = 0;

            double wyjscie1 = 0;
            double wyjscie2 = 0;
            double wyjscie3 = 0;

            double wyjscie4 = 0;
            double wyjscie5 = 0;
            double wyjscie6 = 0;

            double wyjscie7 = 0;
            double wyjscie8 = 0;
            double wyjscie9 = 0;

            double wyjscie10 = 0;
            double wyjscie11 = 0;
            double wyjscie12 = 0;

            //00
            s1 = osobniki[ktoryOsobnik].Parametry[0] + osobniki[ktoryOsobnik].Parametry[1] * xor0 + osobniki[ktoryOsobnik].Parametry[2] * xor0;
            s2 = osobniki[ktoryOsobnik].Parametry[3] + osobniki[ktoryOsobnik].Parametry[4] * xor0 + osobniki[ktoryOsobnik].Parametry[5] * xor0;
            wyjscie1 = 1 / (1 + Math.Pow(Math.E, -1 * s1));
            wyjscie2 = 1 / (1 + Math.Pow(Math.E, -1 * s2));
            s3 = osobniki[ktoryOsobnik].Parametry[6] + osobniki[ktoryOsobnik].Parametry[7] * wyjscie1 + osobniki[ktoryOsobnik].Parametry[8] * wyjscie2;
            wyjscie3 = 1 / (1 + Math.Pow(Math.E, -1 * s3));

            //01
            s4 = osobniki[ktoryOsobnik].Parametry[0] + osobniki[ktoryOsobnik].Parametry[1] * xor0 + osobniki[ktoryOsobnik].Parametry[2] * xor1;
            s5 = osobniki[ktoryOsobnik].Parametry[3] + osobniki[ktoryOsobnik].Parametry[4] * xor0 + osobniki[ktoryOsobnik].Parametry[5] * xor1;
            wyjscie4 = 1 / (1 + Math.Pow(Math.E, -1 * s4));
            wyjscie5 = 1 / (1 + Math.Pow(Math.E, -1 * s5));
            s6 = osobniki[ktoryOsobnik].Parametry[6] + osobniki[ktoryOsobnik].Parametry[7] * wyjscie4 + osobniki[ktoryOsobnik].Parametry[8] * wyjscie5;
            wyjscie6 = 1 / (1 + Math.Pow(Math.E, -1 * s6));

            //10
            s7 = osobniki[ktoryOsobnik].Parametry[0] + osobniki[ktoryOsobnik].Parametry[1] * xor1 + osobniki[ktoryOsobnik].Parametry[2] * xor0;
            s8 = osobniki[ktoryOsobnik].Parametry[3] + osobniki[ktoryOsobnik].Parametry[4] * xor1 + osobniki[ktoryOsobnik].Parametry[5] * xor0;
            wyjscie7 = 1 / (1 + Math.Pow(Math.E, -1 * s7));
            wyjscie8 = 1 / (1 + Math.Pow(Math.E, -1 * s8));
            s9 = osobniki[ktoryOsobnik].Parametry[6] + osobniki[ktoryOsobnik].Parametry[7] * wyjscie7 + osobniki[ktoryOsobnik].Parametry[8] * wyjscie8;
            wyjscie9 = 1 / (1 + Math.Pow(Math.E, -1 * s9));

            //11
            s10 = osobniki[ktoryOsobnik].Parametry[0] + osobniki[ktoryOsobnik].Parametry[1] * xor1 + osobniki[ktoryOsobnik].Parametry[2] * xor1;
            s11 = osobniki[ktoryOsobnik].Parametry[3] + osobniki[ktoryOsobnik].Parametry[4] * xor1 + osobniki[ktoryOsobnik].Parametry[5] * xor1;
            wyjscie10 = 1 / (1 + Math.Pow(Math.E, -1 * s10));
            wyjscie11 = 1 / (1 + Math.Pow(Math.E, -1 * s11));
            s12 = osobniki[ktoryOsobnik].Parametry[6] + osobniki[ktoryOsobnik].Parametry[7] * wyjscie10 + osobniki[ktoryOsobnik].Parametry[8] * wyjscie11;
            wyjscie12 = 1 / (1 + Math.Pow(Math.E, -1 * s12));

            wynik1 = d1 - wyjscie3;
            wynik2 = d2 - wyjscie6;
            wynik3 = d3 - wyjscie9;
            wynik4 = d4 - wyjscie12;


            result += Math.Pow(wynik1,2);
            result += Math.Pow(wynik2,2);
            result += Math.Pow(wynik3,2);
            result += Math.Pow(wynik4,2);

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
            var b_punkt = random.Next(cb.Count); //Lbnch-1 ??

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

        public static void OperatorKrzyzowania(List<Osobnik> osobniki, int Lbnp, int indexOsobnik1, int indexOsobnik2)
        {
            var random = new Random();
            var temporary = new List<int>();
            var b_ciecie = random.Next(osobniki[indexOsobnik1].liczbaChromosomow-1);
            var counter = 0;
            for (int j = b_ciecie+1; j < osobniki[indexOsobnik1].liczbaChromosomow; j++) //czy powinno być do LBNP -1 czy tak jak jest?
            {
                temporary.Add(osobniki[indexOsobnik1].Chromosomy[j]);
                osobniki[indexOsobnik1].Chromosomy[j] = osobniki[indexOsobnik2].Chromosomy[j];
                osobniki[indexOsobnik2].Chromosomy[j] = temporary[counter];
                counter++;
            }

            
        }

        public static Osobnik OperatorHotDeck(List<Osobnik> osobniki, string MinCzyMax)
        {
            var indexNajlepszego = 0;
            if (MinCzyMax == "Max")
            {
                for (int j = 0; j < osobniki.Count; j++)
                {
                    if (osobniki[j].WartoscFunkcjiPrzystosowania > osobniki[indexNajlepszego].WartoscFunkcjiPrzystosowania)
                    {
                        indexNajlepszego = j;
                    }
                }
            }
            else
            {
                for (int j = 0; j < osobniki.Count; j++)
                {
                    if (osobniki[j].WartoscFunkcjiPrzystosowania < osobniki[indexNajlepszego].WartoscFunkcjiPrzystosowania)
                    {
                        indexNajlepszego = j;
                    }
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
