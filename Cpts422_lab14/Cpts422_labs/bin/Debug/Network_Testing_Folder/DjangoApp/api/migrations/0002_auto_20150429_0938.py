# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations
import datetime
from django.utils.timezone import utc


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0001_initial'),
    ]

    operations = [
        migrations.RemoveField(
            model_name='api',
            name='api_json',
        ),
        migrations.AddField(
            model_name='api',
            name='api_oauth',
            field=models.CharField(default=datetime.datetime(2015, 4, 29, 16, 38, 24, 245463, tzinfo=utc), max_length=200),
            preserve_default=False,
        ),
    ]
