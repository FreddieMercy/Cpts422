from .models import API
from django.http import HttpResponse, JsonResponse

def index(request, _id = 1):
	return JsonResponse(API.objects.get(id = _id).api_json, safe=False)
	#return HttpResponse(API.objects.get(id = _id).api_json)


