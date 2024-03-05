import qrcode
import sys
import time as t
import os
import sched


def generate_timestamp_for_qr_code(img):
    timestamp = int(t.time())


    directory = os.path.join(os.path.dirname(os.path.realpath(__file__)))
    directory = os.path.join(directory, "..", "QRcodes")
    
    os.makedirs(directory, exist_ok=True)
    
    finalfilename = os.path.join(directory, f"BUUUUUU_{timestamp}.png")
    img.save(finalfilename)

def qr_code_gen(datastring):
    
    qr = qrcode.QRCode(
        version=1,
        error_correction=qrcode.constants.ERROR_CORRECT_L,
        box_size=10,
        border=4,
    )
    qr.add_data(datastring)
    qr.make(fit=True)
    img = qr.make_image(fill_color="black", back_color="white")
    generate_timestamp_for_qr_code(img)



def generate_qr_code(arg1, arg2, arg3):
    if not arg2:
        qr_code_gen(arg1)
    else:
        qr_code_gen(f"WIFI:T:{arg1};S:{arg2};P:{arg3};;")
    #alkalmazas: app_uri = "myapp://launch"
    #sms: sms_data = "SMSTO:+1234567890:Hello World!"
    #telefonszam: phone_number = "tel:+1234567890"
    '''
    qr = qrcode.QRCode(
        version=1,
        error_correction=qrcode.constants.ERROR_CORRECT_L,
        box_size=10,
        border=4,
    )

    
    qr.add_data(data)
    qr.make(fit=True)

    img = qr.make_image(fill_color="black", back_color="white")

    #t.sleep(2)
    
    timestamp = int(t.time())

    filename = f"BUUUUUU_{timestamp}.png"
    directory = os.path.join(os.path.dirname(os.path.realpath(__file__)))
    directory = os.path.join(directory, "..", "QRcodes")
    print(directory)
    os.makedirs(directory, exist_ok=True)
    
    finalfilename = os.path.join(directory, f"BUUUUUU_{timestamp}.png")
    img.save(finalfilename)
    
    schedule_deletion(finalfilename)
'''




def schedule_deletion(finalfilename):
    scheduler = sched.scheduler(t.time, t.sleep)
    scheduler.enter(5, 1, delete_old_qr_code, argument=(scheduler, 5,finalfilename))
    scheduler.run()

def nothing( sch, dela):
    pass


def delete_old_qr_code(scheduler, delay, finalfilename):
   
    directory = os.path.dirname(os.path.realpath(__file__))
    directory = os.path.join(directory, "..", "QRcodes")
    
    for filename in os.listdir(directory):
        print(f"debug::: {filename}")
        if filename == os.path.basename(finalfilename):             #legutolso qr kodot megmentjuk mert ugye azt nemtorlodki.
            print(f"finalfile: {finalfilename}")
            continue


        if filename.startswith("BUUUUUU_") and filename.endswith(".png"):
            file_path = os.path.join(directory, filename)
            print(f"removed qr-codes: {file_path}")
            os.remove(file_path)

def check_type_of_qr_code(arg0, arg1, arg2, arg3):
    if arg0 == "0":
        generate_qr_code(arg1,arg2,arg3)
    else:
        generate_qr_code(arg1,None,None)        


if __name__ == "__main__":
    #t.sleep(1)
    
    arg0 = sys.argv[1] if len(sys.argv) > 1 else ""
    arg1 = sys.argv[2] if len(sys.argv) > 2 else ""
    arg2 = sys.argv[3] if len(sys.argv) > 3 else ""
    arg3 = sys.argv[4] if len(sys.argv) > 4 else ""
    
    check_type_of_qr_code(arg0, arg1, arg2, arg3)
    #generate_qr_code(arg1, arg2, arg3)
        
    
    