#include <SoftwareSerial.h>

//Message
String incomingMessage = "";

//Output led for debugging
int outPin = 9;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(outPin, OUTPUT);
  digitalWrite(outPin, HIGH);
}

void loop() {
  // put your main code here, to run repeatedly:
  
  readSerialLine();
  //readSerialString();
  gloveHandler(incomingMessage);
  incomingMessage = "";
}

void readSerialLine(){
  if(Serial.available() > 0){
    char byteReceived = Serial.read();
    while(byteReceived != '\n'){
      incomingMessage += byteReceived;
      byteReceived = Serial.read();
    }
    incomingMessage += byteReceived;
    
  }
}

void readSerialString(){
  if(Serial.available()>0){
    byte buff[15];
    Serial.readBytes(buff,15);
    //incomingMessage = String(buff);
  }
}

void gloveHandler(String message){
  if(message != "NA" && message != ""){
    String percentage = getValue(message, ':', 0);
    String controlWord = getValue(message, ':', 1);
    //Palm signal
    char palm = controlWord.charAt(10);
    if(palm == '0')
      digitalWrite(outPin, HIGH);
    else if (palm == '1')
      digitalWrite(outPin, LOW);
  }
  else
    digitalWrite(outPin, HIGH);

}

String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }

  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}

