# ToggleHypervisor
***A small Windows GUI tool to quickly toggle the Windows ```hypervisorlaunchtype``` boot flag (and optionally reboot)***

Useful when you want to be able to alternate between Microsoft's Hyper-V virtualization (used in WSL and Hyper-V Manager) and some third-party virtualization solutions like VMWare Workstation.
 
 Copyright 2023 Jann Emken
 
 ### Features
 - **In the main view:**
     - toggle the hypervisor boot flag
     - optional: reboot immediately
 - **In the details view:**
     - toggle the hypervisor boot flag
     - install the the Hyper-V Hypervisor if not installed
     - set "reboot on toggle" (gets remembered via a settings file)
 
 Uses my other project [FileLogger](https://github.com/q-g-j/FileLogger) for error logging to a file (default location of the file is the same as the EXE).
 
