FROM ubuntu:bionic

RUN apt-get update
RUN apt-get install curl python3.7 python3-pip -y python3-setuptools python3.7-dev python3.7-distutils

RUN update-alternatives --install /usr/bin/python python /usr/bin/python3.7 1
RUN update-alternatives --set python /usr/bin/python3.7
RUN curl -s https://bootstrap.pypa.io/get-pip.py -o get-pip.py && \
    python get-pip.py --force-reinstall && \
    rm get-pip.py

WORKDIR /usr/app
COPY . .
RUN pip install --upgrade pip
RUN pip install --no-cache-dir --default-timeout=900 -r requirements.txt

CMD [ "python", "WordAlignerServer.py" ]