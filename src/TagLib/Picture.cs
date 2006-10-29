/***************************************************************************
    copyright            : (C) 2006 Novell, Inc.
    email                : Aaron Bockover <abockover@novell.com>
 ***************************************************************************/

/***************************************************************************
 *   This library is free software; you can redistribute it and/or modify  *
 *   it  under the terms of the GNU Lesser General Public License version  *
 *   2.1 as published by the Free Software Foundation.                     *
 *                                                                         *
 *   This library is distributed in the hope that it will be useful, but   *
 *   WITHOUT ANY WARRANTY; without even the implied warranty of            *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU     *
 *   Lesser General Public License for more details.                       *
 *                                                                         *
 *   You should have received a copy of the GNU Lesser General Public      *
 *   License along with this library; if not, write to the Free Software   *
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  *
 *   USA                                                                   *
 ***************************************************************************/

using System;
 
namespace TagLib
{
    public enum PictureType
    {
        Other              = 0x00, // A type not enumerated below
        FileIcon           = 0x01, // 32x32 PNG image that should be used as the file icon
        OtherFileIcon      = 0x02, // File icon of a different size or format
        FrontCover         = 0x03, // Front cover image of the album
        BackCover          = 0x04, // Back cover image of the album
        LeafletPage        = 0x05, // Inside leaflet page of the album
        Media              = 0x06, // Image from the album itself
        LeadArtist         = 0x07, // Picture of the lead artist or soloist
        Artist             = 0x08, // Picture of the artist or performer
        Conductor          = 0x09, // Picture of the conductor
        Band               = 0x0A, // Picture of the band or orchestra
        Composer           = 0x0B, // Picture of the composer
        Lyricist           = 0x0C, // Picture of the lyricist or text writer
        RecordingLocation  = 0x0D, // Picture of the recording location or studio
        DuringRecording    = 0x0E, // Picture of the artists during recording
        DuringPerformance  = 0x0F, // Picture of the artists during performance
        MovieScreenCapture = 0x10, // Picture from a movie or video related to the track
        ColouredFish       = 0x11, // Picture of a large, coloured fish
        Illustration       = 0x12, // Illustration related to the track
        BandLogo           = 0x13, // Logo of the band or performer
        PublisherLogo      = 0x14  // Logo of the publisher (record company)
    }

    public interface IPicture
    {
        string MimeType { get; set; }
        PictureType Type { get; set; }
        string Description { get; set; }
        ByteVector Data { get; set; }
    }

    public class Picture : IPicture
    {
        private string      mime_type;
        private PictureType type;
        private string      description;
        private ByteVector  data;
       
        public static Picture CreateFromUri(string uri)
        {
            byte [] fc;
            string filename = null;
            string mimetype = "image/jpeg";
            string ext = "jpg";
            
            try {
                Uri uri_parse = new Uri(uri);
                string path = uri_parse.LocalPath;
                filename = System.IO.Path.GetFileName(path);
                if(filename == String.Empty) {
                    filename = null;
                }
            } catch {
            }
        
            Picture picture = new Picture();
            picture.Data = ByteVector.FromUri(uri, out fc, true);
            
            if(fc.Length >= 4 && (fc[1] == 'P' && fc[2] == 'N' && fc[3] == 'G')) {
                mimetype = "image/png";
                ext = "png";
            }
            
            picture.MimeType = mimetype;
            picture.Type = PictureType.FrontCover;
            picture.Description = filename == null ? ("cover." + ext) : mimetype; 
            
            return picture;
        }
       
        public string MimeType {
            get { return mime_type; }
            set { mime_type = value; }
        }
      
        public PictureType Type {
            get { return type; }
            set { type = value; }
        }
      
        public string Description {
            get { return description; }
            set { description = value; }
        }
      
        public ByteVector Data {
            get { return data; }
            set { data = value; }
        }
    }
}
