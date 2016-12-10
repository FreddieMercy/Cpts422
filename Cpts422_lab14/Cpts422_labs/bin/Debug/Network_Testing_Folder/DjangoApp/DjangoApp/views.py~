from django.shortcuts import render_to_response
from django.http import HttpResponse
from api.models import API
from django.template.response import TemplateResponse
import datetime
import json
import collections

def index(request):
	context=dict(
		year = datetime.datetime.now().year,
		#spotify = collections.OrderedDict(sorted(json.loads(API.objects.get(id = 2).api_json).items())),
		spotify = json.loads(API.objects.get(id = 2).api_keys),
	)
	return TemplateResponse(request, 'Home.html', context)
	
