
int onlinePort = 10;

int onePlayerPort = 13;
int twoPlayerPort = 12;
int threePlayerPort = 11;

void setup() {
  pinMode(onlinePort, OUTPUT);
  pinMode(onePlayerPort, OUTPUT);
  pinMode(twoPlayerPort, OUTPUT);
  pinMode(threePlayerPort, OUTPUT);
  
  Serial.begin(9600);
  Serial.println("Hello, World");
}

int incomingByte = 0;
int playersOnline = 0;

void loop() {
  digitalWrite(onlinePort, HIGH);
  
  if (Serial.available() > 0) {
          // read the incoming byte:
          incomingByte = Serial.read();
     
          playersOnline = incomingByte - 48; // it's an ascii byte, so convert to digit hack :-)
          
          switch (playersOnline)
          {
             case 0:
                digitalWrite(onePlayerPort, LOW);
                digitalWrite(twoPlayerPort, LOW);
                digitalWrite(threePlayerPort, LOW);
                break;
                
             case 1:
                digitalWrite(onePlayerPort, HIGH);
                digitalWrite(twoPlayerPort, LOW);
                digitalWrite(threePlayerPort, LOW);
                break;
                
            case 2:
                digitalWrite(onePlayerPort, HIGH);
                digitalWrite(twoPlayerPort, HIGH);
                digitalWrite(threePlayerPort, LOW);
                break;
                
            default:
                digitalWrite(onePlayerPort, HIGH);
                digitalWrite(twoPlayerPort, HIGH);
                digitalWrite(threePlayerPort, HIGH);    
          }
          
          String playersOnlineHint = "Setting players online to: ";
          String playersOnlineText = String(playersOnline);
          Serial.println(playersOnlineHint + playersOnlineText);
  }
}
