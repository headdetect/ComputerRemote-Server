using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Shell32;

namespace TVRemoteGUI.Windows.Interop {

    public class Video {

        public string Location { get; set; }

        public TimeSpan Length { get; set; }

        public string Name { get; set; }

        public Video ( string location, string name, TimeSpan length ) {
            Location = location;
            Name = name;
            Length = length;
        }

        public Video ( string location ) {
            Location = location;
            Name = Path.GetFileName ( location );

            int index = -1;
            switch ( Path.GetExtension ( location ) ) {

                case "avi":
                    index = 27;
                    break;


            }
            

            Length = FindLength ( location, index );
        }



        public static TimeSpan FindLength ( string location, int index ) {
            try {

                Shell shell = new Shell ();
                var dr = shell.NameSpace ( location );
                var itm = dr.ParseName ( Path.GetFileName ( location ) );

                var prop = dr.GetDetailsOf ( itm, index );

                return TimeSpan.Parse ( prop );

            } catch {
                return new TimeSpan ();
            }
        }
    }


}
