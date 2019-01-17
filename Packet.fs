module Packet
open System
open System.Collections.Generic





type Packet() =
    
    let mutable remote_ep_ = null
    let mutable msg_ = ""
    let mutable c_date_ = 0

    member x.Set(remote_ep, msg) =
        remote_ep_ <- remote_ep
        msg_ <- msg
        c_date_ <- Util.UnixTimeStamp()

    member x.RemoteEndPoint() =
        remote_ep_

    member x.Message() =
        msg_

    member x.CDate() =
        c_date_





type PacketQueue() =

    let lock_obj_ = Object()
    let packet_q_ = Queue<Packet>()


    member x.Enqueue(packet : Packet) =
        lock(lock_obj_) (fun()->
            packet_q_.Enqueue(packet)
            )


    member x.Count() =
        lock(lock_obj_) (fun() ->
            packet_q_.Count
            )
    

    //need try-with
    member x.Dequeue() =
        lock(lock_obj_) (fun()->
            packet_q_.Dequeue()
        )

