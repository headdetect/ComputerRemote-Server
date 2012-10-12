using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVRemote.Utils {

    public class TypeCollection<T> : System.Windows.Forms.ListBox.ObjectCollection {

        private ListBox owner;

        public TypeCollection( ListBox owner )
            : base( owner ) {
            this.owner = owner;
        }

        public new IEnumerator<T> GetEnumerator() {
            while ( base.GetEnumerator().MoveNext() )
                yield return ( T ) owner.Items.GetEnumerator().Current;
        }

        public int Add( T entry ) {
            if ( entry == null )
                throw new ArgumentNullException( "entry", "entry is null." );

            return this.owner.Items.Add( entry );
        }


        public void AddRange( T[] items ) {
            foreach ( var item in items )
                Add( item );
        }

        public void AddRange( List<T> items ) {
            foreach ( var item in items )
                Add( item );
        }

        public new T this[ int index ] {
            get {
                return ( T ) owner.Items[ index ];
            }
            set {
                owner.Items[ index ] = ( T ) value;
            }
        }
    }
}
