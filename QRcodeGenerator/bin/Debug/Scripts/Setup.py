def req():
    import subprocess as s
    print(s.run(["pip","install","-r","requirements.txt"]))



req()
