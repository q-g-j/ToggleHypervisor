# ToggleHypervisor
***A small Windows GUI tool to quickly toggle the Windows ```hypervisorlaunchtype``` boot flag (and optionally reboot)***

Useful if you want to be able to quickly alternate between Microsoft's Hyper-V virtualization (used in WSL and Hyper-V Manager) and third-party virtualization solutions like VMWare Workstation (toggling requires a reboot).
 
 Copyright 2023 Jann Emken
 
 **---- Work in progress ----**<br/>
 *(The program is fully functional, but needs styling.)*
 
 ### Features
 - **In the main view:**
     - toggle the hypervisor boot flag
     - optional: reboot immediately
 - **In the details view:**
     - set the hypervisor boot flag
     - install the the Hyper-V Hypervisor if not installed
     - set "reboot on toggle" (gets remembered via a settings file)
 
 ### Dependencies 
**ToggleHypervisor** uses my other project **[FileLogger](https://github.com/q-g-j/FileLogger)** for error logging to a file (default location of the file is the same as the EXE).<br/>
 It is included as a git submodule. Use the following git commands to fetch it:

```
git clone https://github.com/q-g-j/ToggleHypervisor
cd ToggleHypervisor
git submodule update --init --recursive
```
 
To update all:

```
git pull --recurse-submodules
```
