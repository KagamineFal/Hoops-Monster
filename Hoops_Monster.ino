#include <ESP32Servo.h>             // Library untuk servo pada ESP32
#include <Adafruit_NeoPixel.h>      // Library untuk kontrol LED WS2812

// Pin untuk komponen
const int buttonPin   = 21;
const int sensorPin1  = 23;
const int sensorPin2  = 19;
const int servoPin    = 13;
const int servoPin2   = 12;
const int ledPin      = 22;

// Variabel permainan
int score                = 0;
bool gameStarted         = false;
unsigned long startTime  = 0;
const unsigned long gameDuration = 33000; // 30 detik
bool buttonPressed       = false;
int roundCount           = 0;  // 0 = main round, 1 = bonus

// Threshold untuk bonus round
const int thresholdBonus = 16;

Servo myServo;
Servo myServo2;
Adafruit_NeoPixel strip = Adafruit_NeoPixel(300, ledPin, NEO_GRB + NEO_KHZ800);

// Debounce dan edge detection
unsigned long lastSensorTime = 0;
const unsigned long debounceDelay = 200;
bool lastSensor1State = HIGH;
bool lastSensor2State = HIGH;

// Waktu LED menyala
unsigned long ledTime = 0;

// Debounce tombol
unsigned long lastButtonPress = 0;
const unsigned long buttonDebounceDelay = 500;

void setup() {
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(sensorPin1, INPUT_PULLUP);
  pinMode(sensorPin2, INPUT_PULLUP);

  strip.begin();
  strip.show();

  Serial.begin(115200);
  delay(500);
  Serial.println("GO");

  myServo.attach(servoPin);
  myServo2.attach(servoPin2);
  myServo.write(0);        // Servo kiri posisi tutup
  myServo2.write(180);     // Servo kanan posisi tutup

  lightIdle();             // LED biru redup saat idle
}

void loop() {
  if(Serial.available())
  {
    String dataTerima = Serial.readStringUntil('\r\n');
    dataTerima.trim();
    if(dataTerima.equals("mulai"))
    {
      Serial.println("Game Starting! Prepare yourself.");
      if(!gameStarted) {
        countdown(3);
        startRound();
      }
    }
  }

  // 2) Opsi fallback: cek tombol fisik
  else if (digitalRead(buttonPin) == LOW 
           && !gameStarted && !buttonPressed 
           && (millis() - lastButtonPress > buttonDebounceDelay)) 
  {
    buttonPressed = true;
    lastButtonPress = millis();
    Serial.println("Game Starting! Prepare yourself.");
    countdown(3);
    startRound();
    delay(500);
  }

  // Game loop: cek sensor & timer
  if (gameStarted) {
    unsigned long elapsed = millis() - startTime;
    if (elapsed >= gameDuration) {
      endRound();
    } else {
      processSensors();
    }
  }

  // Setelah LED merah mati, kembalikan ke warna idle (biru redup)
  if (millis() - ledTime > 1000) {
    lightIdle();
  }
}

void startRound() {
  gameStarted = true;
  startTime = millis();
  lastSensorTime = millis();
  lastSensor1State = digitalRead(sensorPin1);
  lastSensor2State = digitalRead(sensorPin2);

  if (roundCount == 0) score = 0;

  if (roundCount == 0) {
    Serial.print("Main Round started! Current Score: ");
  } else {
    Serial.println("Bonus Round started! Current Score: " + String(score));
  }

  myServo.write(90);      // Buka gate kiri 0→90
  myServo2.write(90);     // Buka gate kanan 180→90

  delay(500);
  Serial.println("Gate opened");
}

void processSensors() {
  if (!gameStarted) return;

  unsigned long now = millis();
  bool curr1 = digitalRead(sensorPin1);
  bool curr2 = digitalRead(sensorPin2);

  if (curr1 == LOW && lastSensor1State == HIGH && now - lastSensorTime > debounceDelay) {
    triggerScore(now);
  } else if (curr2 == LOW && lastSensor2State == HIGH && now - lastSensorTime > debounceDelay) {
    triggerScore(now);
  }

  lastSensor1State = curr1;
  lastSensor2State = curr2;
}

void triggerScore(unsigned long eventTime) {
  score++;
  lastSensorTime = eventTime;
  Serial.print("Score: ");
  Serial.println(score);
  lightUpRed();
  ledTime = eventTime;
}

void endRound() {
  gameStarted = false;

  myServo.write(0);        // Tutup gate kiri
  myServo2.write(180);     // Tutup gate kanan

  Serial.println("Gate closed. Checking score...");
  delay(1000);

  if (roundCount == 0) {
    if (score >= thresholdBonus) {
      Serial.println("You Hit The Bonus Round!");
      Serial.print("Congrats! You reached ");
      Serial.print(score);
      Serial.println(" points.");

      roundCount = 1;
      Serial.println("Get ready for the bonus round!");
      delay(1000);
      countdown(3);
      startRound();
    } else {
      Serial.print("Game Over! Final Score: ");
      Serial.println(score);
      resetGame();
    }
  } else {
    Serial.println("You've completed the bonus round!");
    Serial.print("Your Final Score: ");
    Serial.println(score);
    resetGame();
  }
}

void resetGame() {
  Serial.println("Ketik 'mulai' atau tekan tombol untuk restart");
  roundCount = 0;
  buttonPressed = false;
  lightIdle();
  gameStarted = false;
}

void countdown(int sec) {
  for (int i = sec; i > 0; i--) {
    Serial.println(i);
    delay(1000);
  }
  Serial.println("GO!");
}

void lightUpRed() {
  for (int i = 0; i < strip.numPixels(); i++) {
    strip.setPixelColor(i, strip.Color(50, 0, 0));  // Merah redup
  }
  strip.show();
}

void lightIdle() {
  for (int i = 0; i < strip.numPixels(); i++) {
    strip.setPixelColor(i, strip.Color(0, 0, 10));  // Biru redup
  }
  strip.show();
}