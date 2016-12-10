# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0021_auto_20150504_1903'),
    ]

    operations = [
        migrations.AddField(
            model_name='api',
            name='api_keys',
            field=models.TextField(default='s'),
            preserve_default=False,
        ),
    ]
