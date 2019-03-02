using System;
using System.IO;
using System.Text;

namespace Framework.Web
{
    /// <summary>
    /// Removes whitespace from the response content of the page
    /// </summary>
    public class WhiteSpaceResponseFilter : Stream
    {
        #region| Fields |

        private Stream shrink;
        private Func<string, string> filter; 

        #endregion

        #region| Constructor | 

        /// <summary>
        /// Default Constructor 
        /// </summary>
        /// <param name="shrink">Stream</param>
        /// <param name="filter">filter</param>
        public WhiteSpaceResponseFilter(Stream shrink, Func<string, string> filter)
        {
            this.shrink = shrink;
            this.filter = filter;
        }

        #endregion

        #region| Properties |

        /// <summary>
        ///  When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead { get { return true; } }
        
        /// <summary>
        ///  When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek { get { return true; } }
        
        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite { get { return true; } }
        
        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush() { shrink.Flush(); }
       
        /// <summary>
        ///  When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        public override long Length { get { return 0; } }
        
        /// <summary>
        ///  When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        public override long Position { get; set; }

        #endregion

        #region| Methods |
       
        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current
        ///     stream and advances the position within the stream by the number of bytes
        ///     read.
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the
        ///     number of bytes requested if that many bytes are not currently available,
        ///     or zero (0) if the end of the stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return shrink.Read(buffer, offset, count);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">offset</param>
        /// <param name="origin">origin</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return shrink.Seek(offset, origin);
        }
        
        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            shrink.SetLength(value);
        }


        /// <summary>
        ///  Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            shrink.Close();
        }


        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current
        /// stream and advances the current position within this stream by the number
        /// of bytes written.
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string 
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string s = Encoding.Default.GetString(buffer);

            // filter the string
            s = filter(s);

            // write the data to stream 
            byte[] outdata = Encoding.Default.GetBytes(s);
            shrink.Write(outdata, 0, outdata.GetLength(0));
        } 

        #endregion
    }

}
