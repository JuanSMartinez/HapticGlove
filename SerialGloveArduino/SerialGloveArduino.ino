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

  gloveHandler();
  incomingMessage = "";
}

void readSerialLine(){
  if(Serial.available() >0){
    while(Serial.available() < 16) ;
    char byteReceived = Serial.read();
    //Look for starting character
    char i = 0;
    while(byteReceived != 'S' && i < 16){
      byteReceived = Serial.read();
      i ++;
    }
    //Starting character not found, message incomplete
    if(i == 15)
      return;
  
    //Start storing message
    byteReceived = Serial.read();
    while(byteReceived != '\n'){
      incomingMessage.concat(byteReceived);
      byteReceived = Serial.read();
    }
  }
}



void gloveHandler(){
  if(incomingMessage != ""){
    String percentage = getValue(incomingMessage, ':', 0);
    String controlWord = getValue(incomingMessage, ':', 1);
    
    //Palm signal
    char palm = controlWord.charAt(9);
    if (palm == '1')
      digitalWrite(outPin,LOW);
    else
      digitalWrite(outPin,HIGH);
  }
  else
    digitalWrite(outPin,HIGH);

  
 

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

