# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0018_api_api_test'),
    ]

    operations = [
        migrations.RemoveField(
            model_name='api',
            name='api_test',
        ),
    ]
