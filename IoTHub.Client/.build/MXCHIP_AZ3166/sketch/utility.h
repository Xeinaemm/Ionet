#ifndef UTILITY_H
#define UTILITY_H

void EnterIdleState(void);
void ScreenSetup(void);
void InitWiFi(void);
void TurnOffWifi(void);
void WiFiSwitch(void);
void SendForwardEmergencyMessage(const char *);
void ReceiveFromClient(void);
void SendData(void);
void SendToAzure(void);
void SetupDeviceId(void);
void GetTime(char *);

bool EmergencyConnection(void);
void SetupAPServer(void);
void EmergencyMessageLED(void);
void EmergencyConnectLED(void);
void WiFiConnectLED(void);
void AzureConfirmationLED(void);

void parseTwinMessage(DEVICE_TWIN_UPDATE_STATE, const char *);
void MessageToAzure(int, char *, size_t, char *);
void sensorInit(void);
int getInterval(void);

#endif /* UTILITY_H */