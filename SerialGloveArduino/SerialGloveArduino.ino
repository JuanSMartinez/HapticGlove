#include <SoftwareSerial.h>

//Message
String incomingMessage = "";

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  readSerialLine();
  gloveHandler(incommingMessage);
}

void readSerialLine(){
  if(Serial.available() > 0){
    char byteReceived = Serial.read();
    while(byteReceived != '\n'){
      incomingMessage += byteReceived;
      byteReceived = Serial.read();
    }
  }
}

void gloveHandler(String message){
  if(message != "NA"){
    String percentage = getValue(message, ':', 0);
    String controlWord = getValue(message, ':', 1);
  }
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

