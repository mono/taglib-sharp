//
// SimpleTag.cs:
//
// Author:
//   Sebastien Mouy <starwer@laposte.net>
//
// Copyright (C) 2017 Starwer
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagLib.Matroska
{
    /// <summary>
    /// Describes a Matroska Attachment. 
    /// Attachments may be pictures, but also any other content type.
    /// </summary>
    public class Attachment : Picture
    {
        #region Constructors

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="Attachment" /> with no data or values.
        /// </summary>
        public Attachment()
        {
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="Attachment" /> by reading in the contents of a
        ///    specified file.
        /// </summary>
        /// <param name="path">
        ///    A <see cref="string"/> object containing the path of the
        ///    file to read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///    <paramref name="path" /> is <see langword="null" />.
        /// </exception>
        public Attachment(string path) : base (path)
        {
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="Attachment" /> by reading in the contents of a
        ///    specified file abstraction.
        /// </summary>
        /// <param name="abstraction">
        ///    A <see cref="File.IFileAbstraction"/> object containing
        ///    abstraction of the file to read.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///    <paramref name="abstraction" /> is <see langword="null"
        ///    />.
        /// </exception>
        public Attachment(File.IFileAbstraction abstraction) : base(abstraction)
        {
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="Attachment" /> by using the contents of a <see
        ///    cref="ByteVector" /> object.
        /// </summary>
        /// <param name="data">
        ///    A <see cref="ByteVector"/> object containing picture data
        ///    to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///    <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        public Attachment(ByteVector data) : base(data)
        {
        }


        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="Attachment" /> by doing a shallow copy of <see 
        ///    cref="IPicture" />.
        /// </summary>
        /// <param name="picture">
        ///    A <see cref="IPicture"/> object containing picture data
        ///    to convert to an Attachment.
        /// </param>
        public Attachment(IPicture picture) : base(picture)
        {
        }


        #endregion


        #region Property

        /// <summary>
        /// Unique ID representing the file, as random as possible (setting zero will generate automatically a new one).
        /// </summary>
        public ulong UID
        {
            get { return _UID; }
            set
            {
                while (value == 0)
                {
                    value = ((ulong)random.Next()) << 32;
                    value |= (uint)random.Next();
                }
                _UID = value;
            }
        }
        private ulong _UID = (ulong)random.Next();

        ///// <summary>
        ///// Get the Tag representing this attachment
        ///// </summary>
        //public Tag Tag
        //{
        //    get
        //    {
        //        // Lazy declaration
        //        if (_Tag == null)
        //        {
        //            _Tag = new Tag();
        //            _Tag.AttachmentUID = new ulong[] { UID };
        //        }

        //        return _Tag;
        //    }
        //}
        //private Tag _Tag = null;

        #endregion


        #region Statics

        private static Random random = new Random();

        #endregion

    }
}
