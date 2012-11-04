using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TVRemoteGUI.Windows.Utils {

    public class TypeCollection<T> : ListBox.ObjectCollection {

        private readonly ListBox _owner;

        public TypeCollection( ListBox owner )
            : base( owner ) {
            _owner = owner;
        }

        public new IEnumerator<T> GetEnumerator() {
            while ( base.GetEnumerator().MoveNext() )
                yield return ( T ) _owner.Items.GetEnumerator().Current;
        }

        public int Add( T entry ) {
            if ( entry == null )
                throw new ArgumentNullException( "entry", "entry is null." );

            return _owner.Items.Add( entry );
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
                return ( T ) _owner.Items[ index ];
            }
            set {
                _owner.Items[ index ] =  value;
            }
        }
    }
}
