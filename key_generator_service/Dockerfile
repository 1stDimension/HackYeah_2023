FROM python:3.11.5-slim
WORKDIR /usr/src/app

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
WORKDIR /usr/src/app/src
CMD [ "uvicorn", "main:app", "--reload" ]

EXPOSE 8000
