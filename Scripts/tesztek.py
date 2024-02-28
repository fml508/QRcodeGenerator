import time
import pyautogui



def slow_scroll():
    pyautogui.scroll(-(500))  # Negatív értékkel felfelé, pozitív értékkel lefelé görget
    time.sleep(5)
    slow_scroll()


time.sleep(3)
#slow_scroll()

import os
import ctypes

def is_file_in_use(file_path):
    file_in_use = False
    try:
        v_handle = ctypes.windll.kernel32.CreateFileA(file_path, 0x80000000, 0, None, 3, 0x80, None)
        if v_handle == -1:
            file_in_use = True
        else:
            ctypes.windll.kernel32.CloseHandle(v_handle)
    except Exception as e:
        print(f"Error checking if {file_path} is in use: {e}")
    
    print(f"DEBUG FINAL :::: {file_in_use}")
    return file_in_use

def delete_old_qr_code():
    directory = os.path.dirname(os.path.realpath(__file__))
    print(f"directory variable: {directory}")
    
    for filename in os.listdir(directory):
        file_path = os.path.join(directory, filename)
        #print(f"DEBUG :::  {file_path}")
        if filename.startswith("BUUUUUU_") and filename.endswith(".png"):
            try:
                os.remove(file_path)
                print(f"Removed qr-code: {file_path}")
            except Exception as e:
                print(f"Error removing {file_path}: {e}")
    
    print(".,,---:end of the method:---,,.")



delete_old_qr_code()