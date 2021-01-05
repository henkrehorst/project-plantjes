import * as signalR from "@microsoft/signalr";
import * as moment from "moment";

if(document.getElementById("chat-screen") !== null){
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/api/chat")
        .build();

    connection.on("hello", data => {
        console.log(data);
    });

    connection.on("unreadMessageCount", count => {
        console.log(count === 0);
        if(count > 0){
            document.getElementById("message-count").classList.remove("hidden");
            if(count > 9){
                document.getElementById("message-count").innerHTML = `<span>9+</span>`
            }else{
                document.getElementById("message-count").innerHTML = `<span>${count}</span>`
            }
        }else{
            document.getElementById("message-count").classList.add("hidden");
        }
    });
    
    connection.on("receiveMessage", data => {
        //reload chat rooms view
        loadChats();
        
        let message = JSON.parse(data);
        
        if(message.UserId === document.getElementById("message-userid").value){
            if(!document.getElementById("message-view").classList.contains("hidden")) {
                document.getElementById("message-view").innerHTML +=
                    `<p class="mt-2 ml-2 mr-8 p-2 clear-both rounded-md text-base text-black bg-gray-200">
                        ${message.Text}<span class="block mt-1 text-xs font-medium text-gray-500">${new Date().toLocaleTimeString().slice(0,5)}</span>
                    </p>`;
                scrollToBottomMessageView();
                if(!document.getElementById("chat-screen").classList.contains('hidden')) {
                    //set message unread
                    console.log("hoi");
                    connection.send('ReadMessages', message.UserId);
                }
            }
        }
        connection.send("CountUnreadMessages");
    })

    connection.start()
        .then(() => connection.invoke("CountUnreadMessages"));
    
    document.getElementById("send-button").addEventListener('click', () => {
        sendMessage();
    });

    //send message on enter
    document.getElementById("message-box").addEventListener("keyup", (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            sendMessage();
        }
    });
    
    function sendMessage(){
        let message = document.getElementById("message-box").value;
        if(message.trim().length > 0) {
            connection.send("SendMessage", document.getElementById("message-userid").value, message);
            document.getElementById('message-view').innerHTML +=
                `<p class="mt-2 ml-8 mr-2 clear-both rounded-md float-right p-2 text-base text-black bg-gray-200">
                    ${message}<span class="block mt-1 text-xs font-medium text-gray-500">${new Date().toLocaleTimeString().slice(0,5)}</span>
                </p>`;
            scrollToBottomMessageView();
            document.getElementById("message-box").value = "";
        }
    }
    
    async function loadChats(){
        await fetch(window.location.origin + '/api/chat/chats').then(response => {
            if(response.ok){
                if(response.status === 200){
                    response.json().then(data => {
                       let chatRooms = "";
                       data.forEach(chat => {
                           if(chat['unreadMessages'] > 0){
                               chatRooms +=
                                   `<div class="cursor-pointer flex items-center border-b border-black bg-white chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                    <img class="m-2 h-8 w-8 rounded-full chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}" src="${chat['avatar']}" alt="">
                                    <p class="m-0 -mt-1 text-1xl text-black font-bold chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                        ${chat['firstName']}
                                        <span class="block -mt-1 text-xs font-medium text-black chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                            ${moment(chat['lastMessage']).format('HH:mm D-M-Y')}</span>
                                    </p>
                                    <p class="mr-2 mt-0 mb-0 ml-auto h-6 pr-2 pl-2 bg-green-600 rounded-full text-white text-base font-bold flex chat-box" 
                                            data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                        ${chat['unreadMessages']}
                                    </p>
                                </div>`;
                           }else{
                           chatRooms += 
                               `<div class="cursor-pointer flex items-center border-b border-black bg-white chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                    <img class="m-2 h-8 w-8 rounded-full chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}" src="${chat['avatar']}" alt="">
                                    <p class="m-0 -mt-1 text-1xl text-gray-600 font-bold chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                        ${chat['firstName']}
                                        <span class="block -mt-1 text-xs font-medium text-gray-600 chat-box" data-userid="${chat['userId']}" data-unread="${chat['unreadMessages']}">
                                            ${moment(chat['lastMessage']).format('H:M D-M-Y')}
                                        </span>
                                    </p>
                                </div>`;
                       }
                       
                           document.getElementById("chat-rooms").innerHTML = chatRooms;
                            
                           //added chat box function to div
                           Array.from(document.getElementsByClassName('chat-box')).forEach(function (element) {
                               element.addEventListener('click', (e) => {
                                   let userid = e.target.getAttribute('data-userid');
                                   loadChatScreen(userid);
                                   //set message unread
                                   connection.send('ReadMessages', userid);
                               })
                           });
                       })
                    });
                }else{
                    document.getElementById("chat-rooms").innerHTML =
                        "<p class=\"text-black text-base p-4 m-auto\">U heeft nog geen chats, start met ruilen om te gaan chatten.</p>";
                }
            }
        }).catch(e => {
            console.log(e);
        })
    }
    
    document.getElementById("chat-open").addEventListener('click', async () => {
       document.getElementById("chat-screen").classList.remove("hidden");
       showChatLoader();
       await loadChats();
       closeChatLoader();
       if(!document.getElementById("message-view").classList.contains("hidden")){
           let userid = document.getElementById("message-userid").value;
           connection.send('ReadMessages', userid);
       }
    });

    document.getElementById("chat-close").addEventListener('click', () => {
        document.getElementById("chat-screen").classList.add("hidden")
    });

    document.getElementById("chat-room-close").addEventListener('click', () => {
        document.getElementById("chat-screen").classList.add("hidden")
    });
    
    document.getElementById("chat-room-return").addEventListener('click', () => {
        if(document.getElementById("chat-rooms").classList.contains("hidden")) {
            //show chat window elements
            document.getElementById("message-view").classList.add("hidden");
            document.getElementById("chat-controls").classList.add("hidden");
            document.getElementById("chat-header").classList.add("hidden");
            document.getElementById("chat-rooms").classList.remove("hidden");
            document.getElementById("chat-overview-header").classList.remove("hidden");
        }
        loadChats();
    });
    
    
    function showChatLoader(){
        document.getElementById("chat-loader").classList.remove("hidden");
    }

    function closeChatLoader(){
        document.getElementById("chat-loader").classList.add("hidden");
    }
    
    let loadState = false;
    async function loadChatScreen(userid){
        //protect multiple request at the same time 
        if(loadState === true) return false
        loadState = true
        //sent userid in hidden field for sending messages
        document.getElementById("message-userid").value = userid;
        
        showChatLoader();
        if(document.getElementById("message-view").classList.contains("hidden")) {
            //show chat window elements
            document.getElementById("message-view").classList.remove("hidden");
            document.getElementById("chat-controls").classList.remove("hidden");
            document.getElementById("chat-header").classList.remove("hidden");
            document.getElementById("chat-rooms").classList.add("hidden");
            document.getElementById("chat-overview-header").classList.add("hidden");
        }
        
        await fetch(window.location.origin + '/api/chat/messages/' + userid).then(response => {
            if(response.ok){
                response.json().then(data => {
                    console.log(data);
                    document.getElementById("name-place").innerText = data.firstName;
                    document.getElementById("avatar-place").src = data.avatar;
                    //show messages in view
                    let messageOutput = '';
                    data.messages.forEach(element => {
                        if(element.userId === userid){
                            messageOutput = `<p class="mt-2 ml-2 mr-8 p-2 clear-both rounded-md text-base text-black bg-gray-200">
                                ${element.text}<span class="block mt-1 text-xs font-medium text-gray-500">${moment(element.when).format('HH:mm')}</span>
                            </p>` + messageOutput;
                        }else{
                            messageOutput = `<p class="mt-2 ml-8 mr-2 clear-both rounded-md float-right p-2 text-base text-black bg-gray-200">
                                ${element.text}<span class="block mt-1 text-xs font-medium text-gray-500">${moment(element.when).format('HH:mm')}</span>
                            </p>` + messageOutput;
                        }
                    });

                    let messageView = document.getElementById("message-view")
                    messageView.innerHTML = messageOutput;
                    
                    scrollToBottomMessageView();
                })
            }
        }).catch(e => {
            console.log(e);
        })
        closeChatLoader();
        //make messages unread
        connection.send('ReadMessages',userid);
        
        loadState = false
    }
    
    function scrollToBottomMessageView(){
        let messageView = document.getElementById("message-view")
        //scroll to bottom
        messageView.scrollTop = messageView.scrollHeight;
    }
    
    if(document.getElementById("message-button-plant") !== null){
        document.getElementById("message-button-plant").addEventListener('click', () =>{
            let userid = document.getElementById("message-button-plant").dataset.userid;
            if(document.getElementById("chat-screen").classList.contains("hidden")){
                document.getElementById("chat-screen").classList.remove("hidden")
            }
            loadChatScreen(userid);
        })
    }
}