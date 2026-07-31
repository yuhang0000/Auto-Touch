# Auto-Touch
<a>
![License](https://img.shields.io/github/license/yuhang0000/Auto-Touch)
![Platform](https://img.shields.io/badge/platform-Windows-blue)
![.NET](https://img.shields.io/badge/.NET_Framework-4.8.1-purple)
</a>

自動點擊螢幕上的指定位置. 

從 [yuhang0000-Program-Deposit](https://github.com/yuhang0000/yuhang0000-Program-Deposit/tree/main/Repos/Auto%20Touch) 遷移而來, 現在正式成爲獨立應用程式. 

## 食用指南

免安裝可攜式程式, 雙擊程式名即可開啓應用程式. 

### 圖形介面

#### 滑鼠軌道錄製



### 命令列介面

#### 用法

```
Auto Touch [OPTION]

        --help, -h                      顯示該幫助文本
        --version, -ver                 檢查當前版本資訊
        --profile, -p <PATH|NAME>       以指定的預設檔執行
        -x <INT>                        游標 X 軸座標位置
        -y <INT>                        游標 Y 軸座標位置
        --wheel, -w <INT>               滑鼠滾輪滾動距離, 預設为 0
        --action, -a <ACTION>           滑鼠按鈕動作, 預設为 None
        --time, -t <INT|HH:MM:SS>       等待執行, 單位: ms | HH:MM:SS, 預設为立刻執行
        --debug                         偵錯模式, 在當前終端機中列印更多資訊

<ACTION> 可用值: 
        None, MouseLeft, MouseMiddle, MouseRight, MouseXButton1, MouseXButton2
```

#### 使用範例

```
        :: 60 秒後會在 (1920,1080) 滾動 120 格
        Auto Touch.exe -x 1920 -y 1080 --wheel 120 --time 60000
        
        :: 在 (1920,1080) 依序執行滑鼠左鍵和右鍵
        Auto Touch.exe -x 1920 -y 1080 --action MouseLeft,MouseRight
        
        :: 等待個 100ms 後點擊滑鼠左鍵
        Auto Touch.exe -x 1920 -y 1080 --time 100 --action MouseLeft
        
        :: 10 個小時後會在 (1920,1080) 滾動 120 格
        Auto Touch.exe -x 1920 -y 1080 --time 10:00:00
        
        :: 載入預設 #1 並執行
        Auto Touch.exe --profile #1
```

## 注意事項

- 在高權限的應用程式 (如系統設定, 控制面板) 中進行滑鼠軌道錄製和回放時, 需 **以管理員身分執行** 本程式. 
- 建議使用 **Windows 10 1803** 以上版本的作業系統以啟用高精度定時器; 舊版系統仍可正常運行, 但定時精度可能會有所偏差.

## 如何建構

1. 安裝 `Git` 並克隆該倉庫

   ```
   git clone https://github.com/yuhang0000/Auto-Touch.git
   ```

2. 安裝 [Visual Studio 2022 community](https://visualstudio.microsoft.com/zh-hant/downloads/) ;

3. 安裝 [dotNet Framwork 4.8](https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net48) 開發包;

4. 從 `Auto Touch.sln` 打開專案文件;

5. 選擇 `Release` 組態后開始建立應用程式. 

## 系統需求

- windows 7 / 8 / 8.1 / 10 / 11
- 推薦使用 windows 10 1803 以上的版本. 

## 授權協議

[![GPL v3](https://www.gnu.org/graphics/gplv3-127x51.png)](https://www.gnu.org/licenses/gpl-3.0.zh-tw.html)

**Auto Touch** 是一款免費軟體: 你可以依據自由軟體基金會發布的 GNU 通用公共授權的條款來重新分發或者修改它, 你可以選擇使用授權的第 3 版或任何後續版本. 

## 參考資料

請參閲 [lnk](https://github.com/yuhang0000/Auto-Touch/tree/main/lnk) .
