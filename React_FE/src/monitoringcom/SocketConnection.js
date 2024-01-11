import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { MONITORINGCOMSERVICE } from '../commons/hosts';

const SocketConnection = () => {
    const userIdRef = useRef(localStorage.getItem('userId'));
    const connectionRef = useRef(null);

    useEffect(() => {
        userIdRef.current = localStorage.getItem('userId');

        connectionRef.current = new signalR.HubConnectionBuilder()
            .withUrl(MONITORINGCOMSERVICE.backend_api + "/socket-hub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Debug) 
            .build();
        console.log("Current userID" + userIdRef.current)
        
            try {
                connectionRef.current.start();
                console.log("Connection state:", connectionRef.current.state);

                connectionRef.current.invoke("SubscribeToUserTopic", userIdRef.current);
                console.log(`Subscribed to user topic: ${userIdRef.current}`);

                connectionRef.current.on("ReceiveMessage", (message) => {
                    alert(`New message: ${message}`);
                });
            } catch (err) {
                console.error("Error during connection or subscription: ", err);
            }
        

        connectionRef.current.onnegotiationneeded = () => {
            console.log("Negotiation needed.");
        };

        connectionRef.current.onclose = (error) => {
            console.log("Connection closed:", error);
        };


        return () => {
            const disconnectFromSignalR = async () => {
                try {
                    await connectionRef.current.invoke("UnsubscribeFromUserTopic", userIdRef.current);
                    await connectionRef.current.stop();
                    console.log("Disconnected from SignalR hub");
                } catch (err) {
                    console.error("Error during disconnection: ", err);
                }
            };

            disconnectFromSignalR();
        };
    }, [userIdRef]);

    return null;
};

export default SocketConnection;
