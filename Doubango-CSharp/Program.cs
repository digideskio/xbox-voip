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

// http://www.pitorque.de/MisterGoodcat/post/Windows-Phone-7-Mango-Sockets.aspx
// WP7 sockets: http://msdn.microsoft.com/en-us/library/hh202874%28v=VS.92%29.aspx
// WP7 samples: http://msdn.microsoft.com/en-us/library/ff431744%28v=VS.92%29.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doubango.tinySIP;
using Doubango.Tests.SIP;
using Doubango.tinySIP.Headers;
using Doubango.tinyNET;
using Doubango.tinySIP.Transports;
using System.Net;
using Doubango.Tests.Utils;
using Doubango.tinySIP.Sessions;
using Doubango.tinySIP.Events;

namespace Doubango_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test_FSM.DefaultTest();

            //TSIP_TransportUDP transportUdp = new TSIP_TransportUDP("192.168.0.13", TNET_Socket.TNET_SOCKET_PORT_ANY, false, "Sip Tansport using UDP");
            //IPEndPoint remoteEP = TNET_Socket.CreateEndPoint("192.168.0.10", 5060);
           // Int32 count = transportUdp.SendTo(remoteEP, Encoding.UTF8.GetBytes("test"));

            //Test_UriParser.TestUriParser();
            //Test_UriParser.TestMessageParser();

            /*List<TSIP_HeaderVia> headers = TSIP_HeaderVia.Parse("Via: SIP/2.0/tcp 127.0.0.1:5082;branch=z9hG4bKc16be5aee32df400d01015675ab911ba,SIP/2.0/udp 127.0.0.1:5082;branch=z9hG4bKeec53b25db240bec92ea250964b8c1fa;received_port_ext=5081;received=192.168.0.13,SIP/2.0/UDP 192.168.0.12:57121;rport=57121;branch=z9hG4bK1274980921982;received_port_ext=5081;received=192.168.0.12\r\n");
            foreach (TSIP_HeaderVia h in headers)
            {
                Console.WriteLine(h.ToString(true, true, true));
            }*/

            //TSIP_HeaderTo header = TSIP_HeaderTo.Parse("t:    <sip:mamadou@open-ims.test>;tag= 12345\r\n");
            //if (header != null)
            //{
            //    Console.WriteLine(header.ToString(true, true, true));
            //}

            TSIP_Stack sipStack = new TSIP_Stack(
                TSIP_Uri.Create("sip:doubango.org"), 
                "004", 
                TSIP_Uri.Create("sip:004@doubango.org"), 
                "192.168.0.10", 5060
                );
            sipStack.LocalIP = "192.168.0.13";//FIXME

            sipStack.Callback = delegate(TSIP_Event @event)
            {
                switch (@event.Type)
                {
                    case TSIP_Event.tsip_event_type_t.DIALOG:
                        {
                            TSIP_EventDialog eventDialog = (@event as TSIP_EventDialog);
                            break;
                        }

                    case TSIP_Event.tsip_event_type_t.REGISTER:
                        {
                            TSIP_EventRegister eventDialog = (@event as TSIP_EventRegister);
                            break;
                        }

                    default:
                        {
                            return false;
                        }
                }
                return true;
            };
                

            if (sipStack.Start())
            {
                TSIP_SessionRegister register = new TSIP_SessionRegister(sipStack);
                register.Register();
            }

            Console.ReadLine();
        }
    }
}
