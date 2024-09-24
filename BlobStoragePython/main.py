import requests
from bs4 import BeautifulSoup
import json
from datetime import datetime

from azure.data.tables import TableServiceClient
from azure.data.tables._models import UpdateMode

url = 'https://super.walmart.com.mx'
deparments_url = '/content/frutas-y-verduras/120007/'
connection_string = "DefaultEndpointsProtocol=https;AccountName=sampleaccountaz204;AccountKey=rESKM5BX7JoF5X+2LKAHWJ5jH1mhL4ydrBdjWEmBh+9LMt1ynknZlCcK67z5bGImBlKXnHvwYE/++AStLzp4rg==;EndpointSuffix=core.windows.net"


def get_html_from_web():
    headers = {
        "user-agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36",
        "sec-fetch-user": "?1"
    }

    page = requests.get(url + deparments_url, headers=headers)

    return page.text


def get_products_from_html(html_text):
    soup = BeautifulSoup(html_text, "html.parser")

    html_script = soup.find("script", id='__NEXT_DATA__')

    return html_script.text


def extract_product_list(json_text):
    data = json.loads(json_text)

    modules = data['props']['pageProps']['initialTempoData']['data']['contentLayout']['modules']

    category_list = [{'products': module['configs']['productsConfig']['products'],
                      'publishedDate': module['publishedDate'],
                      'title': module['configs']['title']} for module in modules if module['type'] == 'ItemCarousel' and
                     module['configs']['title'] in {'Frutas', 'Verduras', 'Orgánicos y Superfoods'}]

    if len(category_list) > 0:
        return category_list

    return []

def run():
    global date_time

    try:
        date_time = datetime.now().strftime("%m/%d/%Y %H:%M:%S")

        html_text = get_html_from_web()
        json_text = get_products_from_html(html_text)
        category_list = extract_product_list(json_text)
        
        table_service_client = TableServiceClient.from_connection_string(conn_str=connection_string)

        table_client = table_service_client.create_table_if_not_exists(table_name='products')

        for category in category_list:
            for i in range(len(category['products'])):
                product = category['products'][i]
                product_entity = {
                    u'PartitionKey': product['id'],
                    u'RowKey': product['id'],
                    u'Name' : product['name'],
                    u'Category' : category['title'],
                    u'CanonicalURL': product['canonicalUrl'],
                    u'Price': product['price'],
                    u'Image': product['image'],
                    u'Date': date_time
                }

                entity = None
                try:
                    entity = table_client.get_entity(partition_key = product['id'], row_key = product['id']) #, select = 'PartitionKey')
                except Exception as ex:
                    entity = None

                if (entity is None or entity['PartitionKey'] is None):
                    entity = table_client.create_entity(entity=product_entity)
                else:
                    entity = table_client.update_entity(mode=UpdateMode.MERGE, entity=product_entity)


    except Exception as ex:
        print(ex)


def main():
    run()


if __name__ == "__main__":
    main()