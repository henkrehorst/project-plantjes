import * as signalR from "@microsoft/signalr";

if(document.getElementById("chat-overview") !== undefined){
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/api/chat")
        .build();

    connection.on("hello", data => {
        console.log(data);
    });
    
    connection.on("receiveMessage", data => {
        let message = JSON.parse(data);
        
        if(message.UserId === document.getElementById("receiver").value){
            console.log(JSON.parse(data));
            document.getElementById("chat").innerHTML += 
                `<div>
                        <div >
                            <div>
                                <p class="sender ">${message.UserId}</p>
                                <p class="">${message.Text}</p>
                                <span class="">${message.When}</span>
                            </div>
                        </div>
                    </div>`;
        }
    })

    connection.start()
        .then(() => connection.invoke("Hello", "Hello"));
    
    document.getElementById("sendButton").addEventListener('click', () => {
       connection.send("SendMessage", document.getElementById("receiver").value, 
           document.getElementById("message").value).then(r =>
       {console.log(r)}); 
    });
}