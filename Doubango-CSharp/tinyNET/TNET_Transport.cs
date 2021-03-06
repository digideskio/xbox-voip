﻿/*Copyright (C) 2010-2011 Mamadou Diop.
* Copyright (C) 2011 Doubango Telecom <http://www.doubango.org>
*
* Contact: Mamadou Diop <diopmamadou(at)doubango(dot)org>
*	
* This file is part of Open Source Xbox-VoIP Project <http://code.google.com/p/xbox-voip/>
*
* Xbox-VoIP is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*	
* XBox-Voip is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*	
* You should have received a copy of the GNU General Public License
* along with XBox-Voip.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doubango.tinySAK;
using System.Net;
using System.Net.Sockets;

namespace Doubango.tinyNET
{
    public class TNET_Transport : IDisposable
    {
        private String mHost;
        private ushort mPort;
        private TNET_Socket.tnet_socket_type_t mType;
        private String mDescription;
        private TNET_Socket mMasterSocket;
        private Boolean mPrepared;
        private Boolean mStarted;
        private readonly IDictionary<Int64, TNET_Socket> mSockets;

        internal event EventHandler<TransportEventArgs> NetworkEvent;

        public TNET_Transport(String host, ushort port, TNET_Socket.tnet_socket_type_t type, String description)
        {
            mHost = host;
            mPort = port;
            mType = type;
            mDescription = description;

            mSockets = new Dictionary<Int64, TNET_Socket>();

            mMasterSocket = new TNET_Socket(host, port, type);
            if (mMasterSocket != null)
            {
                mMasterSocket.RecvSocketCompleted += this.SocketAsyncEventArgs_Callback;
                mSockets.Add(mMasterSocket.Id, mMasterSocket);
            }
        }

        ~TNET_Transport()
        {
            this.Dispose();
        }

        public virtual void Dispose()
        {

        }

        public String Host
        {
            get { return mHost; }
        }

        public ushort Port
        {
            get { return mPort; }
        }

        public TNET_Socket.tnet_socket_type_t Type
        {
            get { return mType; }
        }

        public String Description
        {
            get { return mDescription; }
        }

        public Boolean IsPrepared
        {
            get { return mPrepared; }
        }

        public Boolean IsStarted
        {
            get { return mStarted; }
        }


        public Boolean Start()
        {
            if (mMasterSocket != null)
            {
                mStarted = true;
                return true;
            }
            return false;
        }

        public Boolean Stop()
        {
            if (mMasterSocket != null)
            {
                mStarted = false;
                return true;
            }
            return false;
        }

        public IntPtr ConnectTo(String host, ushort port, TNET_Socket.tnet_socket_type_t type)
        {
            if (!mStarted)
            {
                TSK_Debug.Error("Transport not started");
                return IntPtr.Zero;
            }

            if (this.Type != type)
            {
                TSK_Debug.Error("Master/destination types mismatch [{0}/{1}]", this.Type, type);
                return IntPtr.Zero;
            }

            EndPoint endpoint = TNET_Socket.CreateEndPoint(host, port);
            if (endpoint != null)
            {
                //var args = new SocketAsyncEventArgs();
                //args.RemoteEndPoint = endpoint;
                //args.Completed += this.SocketAsyncEventArgs_Callback;
                // args.SetBuffer(buffer, 0, buffer.Length);

                return IntPtr.Zero;
            }

            return IntPtr.Zero;
        }


        public Int32 SendTo(Int64 localSocket, EndPoint remoteEP, byte[] buffer)
        {
            if (mSockets.ContainsKey(localSocket))
            {
                TNET_Socket socketFrom = mSockets[localSocket];
                if (socketFrom != null)
                {                    
                    return socketFrom.SendTo(remoteEP, buffer);
                }
            }
            else
            {
                TSK_Debug.Error("Cannot find socket with handle = {0}", localSocket);
            }
            return -1;
        }

        protected Int32 SendTo(EndPoint remoteEP, byte[] buffer)
        {
            return this.SendTo(mMasterSocket.Id, remoteEP, buffer);
        }

        public Boolean GetLocalIpAndPort(Int64 socketId, out String ip, out ushort port)
        {
            if (mSockets.ContainsKey(socketId))
            {
                TNET_Socket socket = mSockets[socketId];
                return socket.GetLocalIpAndPort(out ip, out port);
            }
            return mMasterSocket.GetLocalIpAndPort(out ip, out port);
        }

        public Boolean GetLocalIpAndPort(out String ip, out ushort port)
        {
            return GetLocalIpAndPort(mMasterSocket.Id, out ip, out port);
        }

        private void SocketAsyncEventArgs_Callback(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                // FIXME: cleanup
                return;
            }

            if (NetworkEvent == null)
            {
                return;
            }

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                case SocketAsyncOperation.ReceiveFrom:
                    {
                        TNET_Socket tskSocket = (e.UserToken as TNET_Socket);
                        
                        if (e.Buffer != null)
                        {
                            byte[] bytes = new byte[e.BytesTransferred];
                            Buffer.BlockCopy(e.Buffer, 0, bytes, 0, bytes.Length);
                            TransportEventArgs eargs = new TransportEventArgs(TransportEventArgs.TransportEventTypes.Data, bytes, this, tskSocket.Id);
                            NetworkEvent(this, eargs);
                        }
                        // enqueue another event args
                        tskSocket.ScheduleReceiveFromAsync();
                        break;
                    }
                case SocketAsyncOperation.SendTo:
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Trasport event class
        /// </summary>
        public class TransportEventArgs : TSK_EventArgs
        {
            public enum TransportEventTypes
            {
                Data,
                Closed,
                Error,
                Connected,
                Accepted
            }

            private readonly TransportEventTypes mType;
            private readonly byte[] mData;
            private readonly Object mContext;
            private readonly Int64 mLocalSocketId;

            public TransportEventArgs(TransportEventTypes type, byte[] data, Object context, Int64 localSocketId)
            {
                mType = type;
                mData = data;
                mContext = context;
                mLocalSocketId = localSocketId;
            }

            public TransportEventTypes Type
            {
                get { return mType; }
            }

            public byte[] Data
            {
                get { return mData; }
            }

            public Object Context
            {
                get { return mContext; }
            }

            public Int64 LocalSocketId
            {
                get { return mLocalSocketId; }
            }




        }
    }
}
