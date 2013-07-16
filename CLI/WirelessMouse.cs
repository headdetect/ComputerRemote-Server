using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CLI
{
    public class WirelessMouse
    {
        private readonly UdpClient _mListener;
        private IPEndPoint _mEndPoint;

        private bool _running;

        public int Sensitivity { get; set; }

        public WirelessMouse()
        {
            _mListener = new UdpClient(5051);
            _mEndPoint = new IPEndPoint(IPAddress.Any, 5051);

            Sensitivity = 10;
        }

        public void Start()
        {
            _running = true;

            new Thread(Run).Start();
        }

        public void Stop()
        {
            _running = false;
            _mListener.Close();
        }


        void Run()
        {

            while (_running)
            {
                try
                {
                    byte[] data = _mListener.Receive(ref _mEndPoint);
                    if (data.Length != 9)
                        continue;

                    //Handle by header
                    switch (data[0])
                    {
                        //MouseMovement
                        case 0x01:
                            HandleMovement(data);
                            break;
                        case 0x02:
                            HandleClick(data);
                            break;
                        case 0x03:
                            HandleMouseUp(data);
                            break;
                    }

                    Thread.Sleep(3);
                }
                catch
                {

                }
            }


        }

        private void HandleClick(byte[] data)
        {
            byte whatToClick = data[1];
        }

        private int _prevX = -1,
                    _prevY = -1;



        private void HandleMovement(byte[] data)
        {
            int x = Convert.ToInt32(BitConverter.ToSingle(data, 1));
            int y = Convert.ToInt32(BitConverter.ToSingle(data, 5));

            // Touch up //
            if (x == -1 && y == -1)
            {
                _prevX = -1;
                _prevY = -1;
            }
            else
            {
                // TOUCH DOWN!!!!! //
                if (_prevX == -1 && _prevY == -1)
                {
                    _prevX = x;
                    _prevY = y;
                }
                else
                {

                    // If negitive, going left, else right //
                    int onX = x - _prevX;

                    // If positive, going up, else down //
                    int onY = y - _prevY;

                    if (onX < 0)
                    {
                        _prevX -= Math.Abs(onX);
                    }
                    else
                    {
                        _prevX += Math.Abs(onX);
                    }

                    if (onY < 0)
                    {
                        _prevY -= Math.Abs(onY);
                    }
                    else
                    {
                        _prevY += Math.Abs(onY);
                    }

                    Cursor.SetCursorPos(_prevX, _prevY);
                }
                
            }

        }

        private void HandleMouseUp(byte[] data)
        {
            var pos = Cursor.GetCursorPosition();

            _prevX = pos.X;
            _prevY = pos.Y;
        }
    }
}
