﻿/* Copyright (C) 2010-2011 Mamadou Diop. 
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
using tnet_socket_type_t = Doubango.tinyNET.TNET_Socket.tnet_socket_type_t;
using Doubango.tinyNET;

namespace Doubango.tinySIP.Transports
{
    internal class TSIP_TransportUDP : TSIP_Transport
    {
        internal TSIP_TransportUDP(TSIP_Stack stack, String host, ushort port, Boolean useIPv6, String description)
            : base(stack, host, port, useIPv6 ? tnet_socket_type_t.tnet_socket_type_udp_ipv6 : tnet_socket_type_t.tnet_socket_type_udp_ipv4, description)
        {

        }
        internal TSIP_TransportUDP(TSIP_Stack stack, String host, ushort port, String description)
            : this(stack,host, port, false, description)
        {

        }

        internal TSIP_TransportUDP(TSIP_Stack stack, String description)
            : this(stack, TNET_Socket.TNET_SOCKET_HOST_ANY, TNET_Socket.TNET_SOCKET_PORT_ANY, description)
        {

        }

        internal TSIP_TransportUDP(TSIP_Stack stack, Boolean useIPv6, String description)
            : this(stack, TNET_Socket.TNET_SOCKET_HOST_ANY, TNET_Socket.TNET_SOCKET_PORT_ANY, useIPv6, description)
        {

        }
    }
}
