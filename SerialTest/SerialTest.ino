#include <SoftwareSerial.h>


//Message
String incomingMessage = "";

//Amount of actuators
int motors = 10;

//U/D' control of digital potentiometer
int UD = 12;

//INC control of digital potentiometer
int INC = 13;

//Linear regression variables for supply control( Is = m*Vpot + b
float m = 32.555248527564;
float b = 19.580882300717;

//ADC input for digital potentiometer voltage
int Vpot = A0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial.setTimeout(100);
  pinMode(outPin, OUTPUT);
  digitalWrite(outPin, HIGH);
}

void loop() {
  // put your main code here, to run repeatedly:

  readSerialLine();
  processSerialLine();

  incomingMessage = "";
}


void read2Bytes(){
  if(Serial.available()>0){
    while(Serial.available() < 2) ;
    char byteA = Serial.read();
    char byteB = Serial.read();
    String m = "";
    m.concat(byteA);
    m.concat(byteB);
    if(m == "AB"){
    digitalWrite(outPin, LOW);
  }
  else
    digitalWrite(outPin, HIGH);
  }
}


void readSerialLine(){
  if(Serial.available() > 0){
    while(Serial.available() < 17) ;
    char byteReceived = Serial.read();
    //Look for starting character
    while(byteReceived != 'S')
      byteReceived = Serial.read();
  
    //Start storing message
    byteReceived = Serial.read();
    while(byteReceived != '\n'){
      incomingMessage.concat(byteReceived);
      byteReceived = Serial.read();
    }
  }
}

void processSerialLine(){
  if(incomingMessage != "NA" && incomingMessage != ""){
    String percentage = getValue(incomingMessage, ':', 0);
    String controlWord = getValue(incomingMessage, ':', 1);
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


