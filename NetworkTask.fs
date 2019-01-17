module NetworkTask

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Text
open System.Threading

open Packet
open IniMgr
open LogMgr




type NetworkTask() = 

    [<Literal>] 
    let PACKET_SIZE = 1500
    let mutable socket_ = null
    let mutable local_ep_ = null

    let mutable timer_ = System.Diagnostics.Stopwatch()
    let mutable counter_ = 0




    member x.Open(ip : string, port : int) =

        socket_ <- new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        socket_.Blocking <- false

        if ip.Length = 0 then
            local_ep_ <- IPEndPoint(IPAddress.Any, port)
        else
            local_ep_ <- IPEndPoint(IPAddress.Parse(ip), port)
            
        socket_.Bind(local_ep_);
        timer_.Start()




    member x.HandleEvents(packet_q : PacketQueue) =

        if timer_.ElapsedMilliseconds >= 10000L then 
            LogMgr.Instance.WriteLine(sprintf "Handling packet(s) : %i (queued %i)" counter_ (packet_q.Count()))
            timer_.Restart()
            counter_ <- 0
        try
            if socket_.Available > 0 then
                counter_ <- counter_+ 1
                let mutable buffer = Array.zeroCreate<byte> PACKET_SIZE
                let mutable remote_ep = IPEndPoint(IPAddress.None, 0) :> EndPoint 

                let n_recv = socket_.ReceiveFrom(buffer, &remote_ep)
                let msg = Encoding.UTF8.GetString(buffer, 0, n_recv)

                //make a new packet
                let packet = Packet()
                packet.Set(remote_ep, msg)
                packet_q.Enqueue(packet)
            else
                Thread.Sleep(IniMgr.Instance.FindInt("network_task_idle_sleep_msec").Value)
        with
        | ex -> printfn "%s" ex.Message
                ()



    member x.Close() =
        socket_.Close()
