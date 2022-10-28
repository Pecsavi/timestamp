using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timestamp
{
    public delegate void EsemenykezeloDelegate(object sender, Esemeny esemeny); //Joker fügvény alakja

    public class Esemeny : EventArgs
    { public string esemenyleiras; public DateTime idopont; }

    class Aktivitate
    {

        public static event EsemenykezeloDelegate AllapotvaltozasEsemeny; // ez a Jokerfüggvény példánya
        bool valtozas = false;
        public bool Valtozas
        {
            get { return valtozas; }
            set
            {
                valtozas = value; //akkor indul be az esemény, ha megváltozik a szam mező: 
                                  //Esemeny.esemenyleiras = ("Megváltozott a szam mező!");
                if (value == true)
                {
                    AllapotInaktiv();
                }
                else
                {
                    AllapotAktiv();
                }

            }
        }
        private void AllapotInaktiv()
        {
            if (AllapotvaltozasEsemeny != null) AllapotvaltozasEsemeny(this, new Esemeny() { esemenyleiras = "    inactive at:", idopont = DateTime.Now});
        }
        private void AllapotAktiv()
        {
            if (AllapotvaltozasEsemeny != null) AllapotvaltozasEsemeny(this, new Esemeny() { esemenyleiras = "    active at:", idopont = DateTime.Now });
        }

    }
}
