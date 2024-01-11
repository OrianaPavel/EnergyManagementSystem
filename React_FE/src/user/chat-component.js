// ChatComponent.js
import React, { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr'
import { HOSTUSERSERVICE } from '../commons/hosts';

const ChatComponent = () => {
  const [connection, setConnection] = useState(null);
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [isChatVisible, setIsChatVisible] = useState(false);
  const [currentUser, setCurrentUser] = useState(localStorage.getItem('userId'));
  const [userRole, setCurrentRole] = useState(localStorage.getItem('role') === '0'? 'user' : 'admin');
  const [adminUser, setAdminUser] = useState('admin');
  const [receivedMessageFrom, setReceivedMessageFrom] = useState(null);
  const [noAdminOnline, setNoAdminOnline] = useState(false);
  useEffect(() => {
    const initializeSignalR = async () => {
        const newConnection = new HubConnectionBuilder()
        .withUrl(
          '/chatHub/',
          { 
            //accessTokenFactory: () => localStorage.getItem('token'),
            //headers: { "Authorization": "Bearer " +  localStorage.getItem('token')},
            transport: signalR.HttpTransportType.WebSockets, 
            skipNegotiation: true
          }
        )
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();
  
        await newConnection.start()
        .then(() => {
            console.log("SignalR Connected");
            
            newConnection.invoke('SendUserTypeAndUserId', userRole,currentUser)
              .then(() => console.log(`User type message sent: ${userRole} and userId: ${currentUser}`))
              .catch(err => console.error("Error sending user type message:", err));
        })
        .catch(err => console.error(err));
  
      newConnection.on('receivedMessage', (userType,userId, message) => {
        setReceivedMessageFrom(userId);
        console.log("Message Received " + message);
        setMessages(prevMessages => [...prevMessages, {
          role: userType,
          text: message}]);
      });
      newConnection.on('TypingNotification', (message) => {
        alert(message);
      });
      
      newConnection.on('NoAdminOnline', (errorMessage) => {
        setNoAdminOnline(true);
        console.log(`No admin online: ${errorMessage}`);
      });
  
      setConnection(newConnection);
    };
  
    initializeSignalR();
  }, []);
  

  const sendMessage = async () => {
    if (inputMessage.trim() !== '') {
      try {
        console.log("Invoking SendMessage...");
        var sendMsgTo = currentUser;
        if(userRole === 'admin') 
          sendMsgTo = receivedMessageFrom;
        await connection.invoke('SendMessage', userRole,sendMsgTo, inputMessage);
        setMessages(prevMessages => [...prevMessages, {
          role: userRole,
          text: inputMessage}]);
        console.log("SendMessage invoked successfully!");
      } catch (error) {
        console.error("Error invoking SendMessage:", error);
      }
  
      setInputMessage('');
    }
  };
  

  const canSendMessage = () => {
    return currentUser !== adminUser || messages.length > 0;
  };

  const toggleChatVisibility = () => {
    setIsChatVisible(!isChatVisible);
  };
  const handleOnFocus = () => { 
    var sendMsgTo = currentUser;
        if(userRole === 'admin') 
          sendMsgTo = receivedMessageFrom;
        if(receivedMessageFrom === null)
          return;
      try{
        connection.invoke("Typing",sendMsgTo,userRole);
      }catch (error) {
        console.error("Error invoking SendMessage:", error);
      }
  };
  return (
    <div>
      <div className={`chat-container ${noAdminOnline ? 'no-admin-online' : ''}`}>
        <div className="chat-header" onClick={toggleChatVisibility}>
          Chat
        </div>
        <div className="chat-body">
          {noAdminOnline && <div className="no-admin-message">Sorry, no admin is currently online.Try again later!</div>}
          <ul>
            {messages.map((msg, index) => (
              <li key={index}>{msg.role}: {msg.text}</li>
            ))}
          </ul>
          <div className="input-section">
            <input
              type="text"
              onFocus={handleOnFocus}
              value={inputMessage}
              onChange={(e) => setInputMessage(e.target.value)}
            />
            <button onClick={sendMessage} disabled={!canSendMessage()}>
              Send
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatComponent;
