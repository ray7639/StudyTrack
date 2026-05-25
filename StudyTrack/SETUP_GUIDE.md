# StudyTrack - Setup Guide

## Quick Start in Visual Studio

### Step 1: Create New MAUI Project

1. Open Visual Studio 2022
2. File > New > Project
3. Search for ".NET MAUI App"
4. Name it "StudyTrack"
5. Click Create

### Step 2: Replace Project Files

1. **Delete** the default files from the new project:
   - Views folder (if any)
   - ViewModels folder (if any)  
   - MainPage.xaml and MainPage.xaml.cs

2. **Copy ALL files** from this StudyTrack-MAUI folder into your new project folder

3. **Replace** StudyTrack.csproj with the one provided

### Step 3: Restore NuGet Packages

In Visual Studio:
- Right-click Solution > **Restore NuGet Packages**
- Or in Terminal:
```bash
dotnet restore
```

### Step 4: Build the Project

1. Build > Build Solution (Ctrl+Shift+B)
2. Fix any namespace errors if needed
3. Build should succeed

### Step 5: Run the App

1. Select **Android Emulator** from dropdown
   - Or connect physical Android device

2. Press **F5** or click **Run**

3. App should launch showing the Splash Screen

## Building APK for Submission

### Method 1: Using Visual Studio (Recommended)

1. Set configuration to **Release**
   - Configuration Manager > Active Solution Configuration > Release

2. Right-click project > **Publish**

3. Select **Android** > **Ad Hoc**

4. Click **Publish**

5. APK will be created in:
   ```
   bin/Release/net8.0-android/publish/
   ```

### Method 2: Using Command Line

```bash
# Navigate to project folder
cd StudyTrack

# Build Release APK
dotnet publish -f net8.0-android -c Release

# Find APK in:
# bin/Release/net8.0-android/publish/com.studytrack.app-Signed.apk
```

## Testing Checklist

Before submitting, test these features:

- [ ] App launches without crashes
- [ ] User can register new account
- [ ] User can login
- [ ] Dashboard shows statistics
- [ ] Can create new task
- [ ] Can edit task
- [ ] Can delete task
- [ ] Can mark task complete
- [ ] Can add subject
- [ ] Can create notes
- [ ] Profile shows correct info
- [ ] About page displays
- [ ] Navigation works smoothly
- [ ] APK installs on physical device

## Common Issues & Solutions

### Issue: Build Fails - Missing References

**Solution:**
```bash
dotnet restore
dotnet clean
dotnet build
```

### Issue: Android Emulator Won't Start

**Solution:**
- Enable Hyper-V in Windows Features
- Or use physical device via USB debugging

### Issue: SQLite Database Errors

**Solution:**
- Check that sqlite-net-pcl is installed (v1.9.172)
- Verify SQLitePCLRaw packages are installed

### Issue: App Crashes on Startup

**Solution:**
- Check that all services are registered in MauiProgram.cs
- Verify all View/ViewModel are registered
- Check namespaces match

## Customization Tips

### Change App Colors

Edit `Resources/Styles/Colors.xaml`:
```xml
<Color x:Key="Primary">#YOUR_COLOR</Color>
```

### Change App Name

Edit `StudyTrack.csproj`:
```xml
<ApplicationTitle>Your App Name</ApplicationTitle>
```

### Add App Icon

1. Create icon image (512x512 PNG)
2. Add to `Resources/AppIcon/`
3. Update reference in .csproj

## GitHub Repository Setup

```bash
git init
git add .
git commit -m "Initial commit: StudyTrack MAUI app"
git branch -M main
git remote add origin YOUR_REPO_URL
git push -u origin main
```

## Project Structure Reference

```
StudyTrack/
├── Models/              # Data models (User, Task, Subject, Note)
├── Views/               # XAML pages (10 screens)
├── ViewModels/          # MVVM view models
├── Services/            # Database and auth services
├── Converters/          # Value converters
├── Resources/
│   ├── Styles/          # Colors and styles
│   ├── Fonts/           # Custom fonts
│   └── Images/          # App images
├── App.xaml             # Application resources
├── AppShell.xaml        # Shell navigation
├── MauiProgram.cs       # Dependency injection
└── StudyTrack.csproj    # Project configuration
```

## Grading Rubric Coverage

| Criteria | Points | How It's Covered |
|----------|--------|------------------|
| UI Design | 20 | Modern blue/white theme, cards, rounded buttons, gradients |
| Navigation | 20 | Shell TabBar, proper routing, back navigation |
| Performance | 15 | Async methods, efficient data loading, no blocking operations |
| UI Components | 20 | DatePicker, SearchBar, CollectionView, modals, custom styles |
| Functionality | 15 | Full CRUD, validation, smooth transitions, error handling |
| Documentation | 10 | This guide, README, inline comments, About page |
| **TOTAL** | **100** | |

## Support

For issues, check:
1. README.md - Full documentation
2. This SETUP_GUIDE.md - Step-by-step instructions
3. Code comments - Inline documentation

## Final Submission Checklist

- [ ] APK file builds successfully
- [ ] APK tested on physical device or emulator
- [ ] GitHub repository created
- [ ] README.md included
- [ ] Screenshots folder added
- [ ] All features working
- [ ] No crashes or errors
- [ ] Code is clean and commented

Good luck with your final project! 🎓📚
