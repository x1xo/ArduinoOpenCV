#define echoPin 2
#define trigPin 3 
long duration;
int distance; 
int triggerDistance = 50;

void setup()
{
	pinMode(trigPin, OUTPUT);
	pinMode(echoPin, INPUT);
	Serial.begin(9600);
  Serial.setTimeout(0);
	delay(500);
}

void loop()
{
	// digitalWrite(trigPin, LOW);
	// delayMicroseconds(2);
	digitalWrite(trigPin, HIGH);
	delayMicroseconds(10);

	digitalWrite(trigPin, LOW);

	duration = pulseIn(echoPin, HIGH);
	distance = duration * 0.0355 / 2; 
  if(distance < triggerDistance) {
    Serial.println(1);
  }
  
  // Serial.print(distance);
  // Serial.print(" - ");
  // Serial.println(triggerDistance);

  String di = Serial.readString();
  if(di.toInt() != 0){
    triggerDistance = di.toInt();
  }

	delay(50);
}
